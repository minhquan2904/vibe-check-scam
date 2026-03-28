using api_vibe.Models;

namespace api_vibe.Services.Impls;

public class StatementProcessingService : IStatementProcessingService
{
    public List<TransactionItem> ExtractTransactions(string rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
            return new List<TransactionItem>();

        // Phân tách đơn giản qua dòng. Ở logic thực tế có thể dùng regex cho đúng pattern sao kê.
        var lines = rawText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var transactions = new List<TransactionItem>();

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.Length > 5) // Ignore lines that are too short to be a transaction
            {
                transactions.Add(new TransactionItem { RawContent = trimmed });
            }
        }

        return transactions;
    }
}
