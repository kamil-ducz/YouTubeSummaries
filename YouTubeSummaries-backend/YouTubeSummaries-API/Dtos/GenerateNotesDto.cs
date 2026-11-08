namespace YouTubeSummaries;

public class GenerateNotesDto
{
    public string Transcript { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public string Length { get; set; } = "short";
    public string Style { get; set; } = "simple";
}
