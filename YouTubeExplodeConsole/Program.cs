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

var trackManifest = await ytclient.Videos.ClosedCaptions.GetManifestAsync(videoUrl ?? "");
var trackInfo = trackManifest.TryGetByLanguage("en") ?? trackManifest.Tracks.FirstOrDefault();

var captionTrack = await ytclient.Videos.ClosedCaptions.GetAsync(trackInfo);

Console.WriteLine("Transcript language: " + trackInfo.Language.Name);

Console.WriteLine("Transcription content: ");
foreach (var caption in captionTrack.Captions)
{
    Console.WriteLine(caption);
}

var manifestJson = JsonConvert.SerializeObject(trackManifest);
var captionJson = JsonConvert.SerializeObject(captionTrack);

var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("http://localhost:11434");

var request = new
{
    model = "minimax-m3:cloud",
    prompt = "Short casual notes from this transcript:\n\n" + captionJson,
    stream = false
};

var response = await httpClient.PostAsJsonAsync("api/generate", request);
var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();
Console.WriteLine(result?.Response);

var relative = Path.Combine("..", "..", "..", "notes");
var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relative));
if (!string.IsNullOrEmpty(path))
{
    Directory.CreateDirectory(path);
}
File.WriteAllText(Path.Combine(path, "note.md"), result?.Response);