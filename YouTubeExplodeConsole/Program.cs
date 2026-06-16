Console.WriteLine("Let's see how YouTubeExplode serves to get transcription from videos");
Console.WriteLine("Please type YT video URL: ");
string? videoUrl = Console.ReadLine();

YoutubeClient ytclient = new YoutubeClient();

var trackManifest = await ytclient.Videos.ClosedCaptions.GetManifestAsync(videoUrl ?? "");
var trackInfo = trackManifest.TryGetByLanguage("en") ?? trackManifest.Tracks.FirstOrDefault();

var captionTrack = await ytclient.Videos.ClosedCaptions.GetAsync(trackInfo);

Console.WriteLine("Transcript language: " + trackInfo.Language.Name);

Console.WriteLine("Transcription content: ");
foreach(var caption in captionTrack.Captions)
{
    Console.WriteLine(caption);
}


