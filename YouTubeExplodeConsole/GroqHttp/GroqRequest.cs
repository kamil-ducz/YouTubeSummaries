// Groq API documentation:
// https://console.groq.com/docs/api-reference#chat-create

using System.Collections.Generic;
using System.Text.Json.Serialization;


public class GroqRequest
{
    [JsonPropertyName("messages")]
    public List<MessageObject>? Messages { get; set; }
    [JsonPropertyName("model")]
    public string? Model { get; set; }
    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }
}

public class MessageObject
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
    [JsonPropertyName("role")]
    public string? Role { get; set; }    
}