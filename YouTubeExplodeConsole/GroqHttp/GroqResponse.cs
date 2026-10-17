// Groq API documentation:
// https://console.groq.com/docs/api-reference#chat-create

using System.Collections.Generic;
using System.Text.Json.Serialization;

public class GroqResponse
{
    [JsonPropertyName("choices")]
    public List<Choice>? Choices { get; set; }
}

public class Choice
{
    [JsonPropertyName("message")]
    public ResponseMessage? Message { get; set; }
}

public class ResponseMessage
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}