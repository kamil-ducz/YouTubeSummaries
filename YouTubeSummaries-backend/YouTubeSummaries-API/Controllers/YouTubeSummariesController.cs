using Microsoft.AspNetCore.Mvc;
using System.Text;
using YoutubeExplode;

namespace YouTubeSummaries;

[Route("api/[controller]")]
[ApiController]
public class YouTubeSummariesController : ControllerBase
{
    private readonly YoutubeClient _ytClient = new YoutubeClient();
    private readonly ISummaryService _summaryService;

    public YouTubeSummariesController(ISummaryService summaryService)
    {
        _summaryService = summaryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTranscript([FromQuery] string videoUrl)
    {
        if (string.IsNullOrWhiteSpace(videoUrl) || !Uri.IsWellFormedUriString(videoUrl, UriKind.Absolute))
        {
            return BadRequest("Invalid URL");
        }
        try
        {
            var track = await _ytClient.Videos.GetAsync(videoUrl);
            var trackTitle = track.Title;

            StringBuilder sanitizedTitle = HelperMethods.SanitizeFilename(trackTitle);

            var trackManifest = await _ytClient.Videos.ClosedCaptions.GetManifestAsync(videoUrl);
            var trackInfo = trackManifest.TryGetByLanguage("en") ?? trackManifest.Tracks.FirstOrDefault();

            if (trackInfo is null)
            {
                return BadRequest("There are no captions for this video");
            }

            var trackCaption = await _ytClient.Videos.ClosedCaptions.GetAsync(trackInfo);

            var captionsList = trackCaption.Captions.Select(c => new
            {
                Text = c.Text,
                StartInSeconds = c.Offset.TotalSeconds
            }).ToList();

            return Ok(new
            {
                Title = trackTitle,
                Language = trackInfo.Language,
                Captions = captionsList
            });

        }
        catch (Exception ex)
        {
            return BadRequest($"There was an error in communication with YouTube {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> MakeNotesFromCaptions([FromBody] GenerateNotesDto request)
    {
        if(string.IsNullOrWhiteSpace(request.Transcript))
        {
            return BadRequest("Transcription can't be empty");
        }

        try
        {
            var notes = await _summaryService.GenerateNotesAsync(
                request.Transcript,
                request.Language,
                request.Length,
                request.Style
            );

            return Ok(new { Notes = notes });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error during generating notes: {ex.Message}");
        }
    }
}