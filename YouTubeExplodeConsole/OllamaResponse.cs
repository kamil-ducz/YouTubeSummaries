using System.Text.Json.Serialization;

internal class OllamaResponse
{
    public string? Model { get; set; }
    [JsonPropertyName("response")]
    public string? Response { get; set; }
}