using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using YoutubeExplode;

Console.WriteLine("Let's see how YouTubeExplode serves to get transcription from videos");
Console.WriteLine("Please type YT video URL: ");
string? videoUrl = Console.ReadLine();

YoutubeClient ytclient = new YoutubeClient();

// TODO handle exception, when given URL is invalid or doesn't exist
var trackManifest = await ytclient.Videos.ClosedCaptions.GetManifestAsync(videoUrl ?? "");
var track = await ytclient.Videos.GetAsync(videoUrl ?? "");
var trackTitle = track.Title;

var trackInfo = trackManifest.TryGetByLanguage("en") ?? trackManifest.Tracks.FirstOrDefault();
if(trackInfo is null)
{
    Console.WriteLine("There are no captions for this video.");
    Console.ReadLine();
    throw new Exception();
}
var trackCaption = await ytclient.Videos.ClosedCaptions.GetAsync(trackInfo);

Console.WriteLine("Transcript language: " + trackInfo.Language.Name);

Console.WriteLine("Transcription content: ");
foreach (var caption in trackCaption.Captions)
{
    Console.WriteLine(caption);
}

var manifestJson = JsonConvert.SerializeObject(trackManifest);
var captionJson = JsonConvert.SerializeObject(trackCaption);

var httpClient = new HttpClient();
// TODO extract address somewhere to not harcode it here
httpClient.BaseAddress = new Uri("http://localhost:11434");

var request = new
{
    model = "minimax-m3:cloud",
    prompt = "Short casual notes from this transcript:\n\n" + captionJson,
    stream = false
};
// TODO handle exception, when there is no local Ollama at 11434
var response = await httpClient.PostAsJsonAsync("api/generate", request);
var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();
Console.WriteLine(result?.Response);

var relative = Path.Combine("..", "..", "..", "notes");
var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relative));
if (!string.IsNullOrEmpty(path))
{
    Directory.CreateDirectory(path);
}
File.WriteAllText(Path.Combine(path, trackTitle + ".md"), result?.Response);