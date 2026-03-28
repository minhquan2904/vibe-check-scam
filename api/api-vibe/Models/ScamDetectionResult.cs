namespace api_vibe.Models;

public class ScamDetectionResult
{
    public bool IsScam { get; set; }
    public double ConfidenceScore { get; set; }
    public string Reason { get; set; } = string.Empty;
}
