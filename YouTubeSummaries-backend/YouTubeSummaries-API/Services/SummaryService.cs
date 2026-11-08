using System.Security.Cryptography;

namespace YouTubeSummaries;

public interface ISummaryService
{
    public Task<string> GenerateNotesAsync(string transcript, string language, string length, string style);
}
internal class SummaryService : ISummaryService
{
    private readonly HttpClient _httpClient;

    public SummaryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GenerateNotesAsync(string transcript, string language, string length, string style)
    {
        string systemInstruction = $"Make notes from YouTube transcription" +
                                   $"Use language: {language}" +
                                   $"Lenght of notes: {length}" +
                                   $"Use style: {style}";

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Keys.GroqAPISecret);

        var groqRequest = new GroqRequest
        {
            Model = "openai/gpt-oss-120b",
            Messages = new List<MessageObject>
                {
                    new MessageObject { Role = "system", Content = systemInstruction },
                    new MessageObject { Role = "user", Content = transcript }
                },
            Temperature = 0.3
        };

        var groqResponse = await _httpClient.PostAsJsonAsync(Paths.ChatCompletions, groqRequest);

        if (!groqResponse.IsSuccessStatusCode)
        {
            var error = await groqResponse.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Groq API failed: {error}");
        }

        var result = await groqResponse.Content.ReadFromJsonAsync<GroqResponse>();
        return result?.Choices?.FirstOrDefault()?.Message?.Content ?? "Couldn't generate notes.";

    }
}

