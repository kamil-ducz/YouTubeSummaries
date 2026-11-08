using System.Text;

namespace YouTubeSummaries;

internal static class HelperMethods
{
    internal static StringBuilder SanitizeFilename(string trackTitle)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        StringBuilder sanitizedTitle = new StringBuilder();
        foreach (var character in trackTitle)
        {
            if (!invalidChars.Contains(character))
            {
                sanitizedTitle.Append(character);
            }
        }

        return sanitizedTitle;
    }
}