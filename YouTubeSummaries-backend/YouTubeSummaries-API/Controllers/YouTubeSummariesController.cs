using Microsoft.AspNetCore.Mvc;
using YoutubeExplode;

namespace YouTubeSummaries;

[Route("api/[controller]")]
[ApiController]
public class YouTubeSummariesController : ControllerBase
{
    private readonly YoutubeClient _ytClient = new YoutubeClient();
    private readonly ISummaryService _summaryService;
    private readonly ITranscriptService _transcriptService;

    public YouTubeSummariesController(ISummaryService summaryService, ITranscriptService transcriptService)
    {
        _summaryService = summaryService;
        _transcriptService = transcriptService;
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
            var result = await _transcriptService.GetTranscriptAsync(videoUrl);
            return Ok(result);
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