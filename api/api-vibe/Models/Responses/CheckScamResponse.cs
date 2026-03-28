namespace api_vibe.Models.Responses;

public class CheckScamResponse
{
    public List<TransactionItem> Transactions { get; set; } = new();
    public ScamDetectionResult ScamAnalysis { get; set; } = new();
}
