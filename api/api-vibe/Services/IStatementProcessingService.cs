using api_vibe.Models;

namespace api_vibe.Services;

public interface IStatementProcessingService
{
    List<TransactionItem> ExtractTransactions(string rawText);
}
