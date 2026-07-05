using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using YoutubeExplode;
using YouTubeExplodeConsole.Config;

Console.WriteLine("Let's see how YouTubeExplode serves to get transcription from videos");
Console.WriteLine("Please type YT video URL: ");
string? videoUrl = Console.ReadLine();

YoutubeClient ytclient = new YoutubeClient();

if (!Uri.IsWellFormedUriString(videoUrl, UriKind.Absolute))
{
    Console.WriteLine("You need to paste proper URL.");
    Environment.Exit(0);
}
var track = await ytclient.Videos.GetAsync(videoUrl);
var trackTitle = track.Title;
StringBuilder sanitizedTitle = SanitizeFilename(trackTitle);

var trackManifest = await ytclient.Videos.ClosedCaptions.GetManifestAsync(videoUrl);

var trackInfo = trackManifest.TryGetByLanguage("en") ?? trackManifest.Tracks.FirstOrDefault();
if (trackInfo is null)
{
    Console.WriteLine("There are no captions for this video.");
    Environment.Exit(0);
}
var trackCaption = await ytclient.Videos.ClosedCaptions.GetAsync(trackInfo);

Console.WriteLine("Transcript language: " + trackInfo.Language.Name);

Console.WriteLine("Transcription content: ");
foreach (var caption in trackCaption.Captions)
{
    Console.WriteLine(caption);
}

var httpClient = new HttpClient();
// IMPORTANT 
// Groq API key is needed below after "Bearer" on the right
// I have it in Visual Studio 18 folder, you should have your own secret
httpClient.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "");
httpClient.DefaultRequestHeaders.Add("User-Agent", "YouTubeExplodeConsoleApp/1.0");

string plainTextTranscript = string.Join(" ", trackCaption.Captions.Select(c => c.Text));

var manifestNotesRequest = new GroqRequest
{
    Model = "llama-3.3-70b-versatile",
    Messages = new List<MessageObject>
        {
            new MessageObject
            {
                Role = "system",
                Content = "Make casual notes from the YouTube transcription, try to keep them short"
            },
            new MessageObject
            {
                Role = "user",
                Content = plainTextTranscript
            }
        },
    Temperature = 0.3
};

var manifestNotesResponse = await httpClient.PostAsJsonAsync(Paths.ChatCompletions, manifestNotesRequest);
if (!manifestNotesResponse.IsSuccessStatusCode)
{
    var error = await manifestNotesResponse.Content.ReadAsStringAsync();
    Console.WriteLine($"Request failed ({(int)manifestNotesResponse.StatusCode}): {error}");
    Environment.Exit(0);
}

var manifestNotesResult = await manifestNotesResponse.Content.ReadFromJsonAsync<GroqResponse>();


var relative = Path.Combine("..", "..", "..", "notes");
var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relative));
if (!string.IsNullOrEmpty(path))
{
    Directory.CreateDirectory(path);
}
File.WriteAllText(Path.Combine(path, sanitizedTitle + ".md"), manifestNotesResult?.Choices?.FirstOrDefault()?.Message?.Content);

static StringBuilder SanitizeFilename(string trackTitle)
{
    var invalidChars = Path.GetInvalidFileNameChars();
    StringBuilder sanitizedTitle = new StringBuilder();
    foreach (var character in trackTitle)
    {
        if (!invalidChars.Contains(character))
        {
            sanitizedTitle.Append(character);
        }
    }

    return sanitizedTitle;
}