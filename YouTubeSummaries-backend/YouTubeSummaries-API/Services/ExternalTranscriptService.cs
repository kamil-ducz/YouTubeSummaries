using System.Text.Json.Serialization;

namespace YouTubeSummaries;

public interface ITranscriptService
{
    Task<TranscriptResponseDto> GetTranscriptAsync(string videoUrl);
}

public class ExternalTranscriptService : ITranscriptService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public ExternalTranscriptService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["TranscriptApi:ApiKey"]
                  ?? throw new InvalidOperationException("TranscriptApi:ApiKey missing in configuration.");
    }

    public async Task<TranscriptResponseDto> GetTranscriptAsync(string videoUrl)
    {
        videoUrl = videoUrl.Trim();
        var encodedUrl = Uri.EscapeDataString(videoUrl);

        var oEmbedUrl = "https://www.youtube.com" + "/oembed?url=" + encodedUrl + "&format=json";
        var apiUrl = "https://api.supadata.ai" + "/v1/youtube/transcript?url=" + encodedUrl;

        try
        {
            var oEmbedData = await _httpClient.GetFromJsonAsync<OEmbedDto>(oEmbedUrl);
            string title = oEmbedData?.Title ?? "YouTube Video";

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Add("x-api-key", _apiKey);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"API transcription error: ({response.StatusCode}): {errorBody}");
            }

            var apiResult = await response.Content.ReadFromJsonAsync<SupadataResponseDto>();
            if (apiResult?.Content == null || !apiResult.Content.Any())
            {
                throw new Exception("No captions for this movie.");
            }

            var captions = apiResult.Content.Select(c => new CaptionItemDto
            {
                Text = c.Text,
                StartInSeconds = c.Offset / 1000.0
            }).ToList();

            return new TranscriptResponseDto
            {
                Title = title,
                Language = apiResult.Lang ?? "en",
                Captions = captions
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"DEBUG -> oEmbedUrl=[{oEmbedUrl}] | apiUrl=[{apiUrl}] | Error: {ex.Message} | Trace: {ex.StackTrace}");
        }
    }

}

public class TranscriptResponseDto
{
    public string Title { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public List<CaptionItemDto> Captions { get; set; } = new();
}

public class CaptionItemDto
{
    public string Text { get; set; } = string.Empty;
    public double StartInSeconds { get; set; }
}

internal class OEmbedDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}

internal class SupadataResponseDto
{
    [JsonPropertyName("lang")]
    public string Lang { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public List<SupadataSegmentDto> Content { get; set; } = new();
}

internal class SupadataSegmentDto
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("offset")]
    public double Offset { get; set; }
}
