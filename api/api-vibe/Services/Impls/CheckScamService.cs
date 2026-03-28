using api_vibe.Infrastructure.Gemini;
using api_vibe.Models;
using api_vibe.Models.Requests;
using api_vibe.Models.Responses;

namespace api_vibe.Services.Impls;

public class CheckScamService : ICheckScamService
{
    private readonly IGeminiClient _geminiClient;
    private readonly IStatementProcessingService _statementService;
    private readonly ILogger<CheckScamService> _logger;

    public CheckScamService(
        IGeminiClient geminiClient, 
        IStatementProcessingService statementService,
        ILogger<CheckScamService> logger)
    {
        _geminiClient = geminiClient;
        _statementService = statementService;
        _logger = logger;
    }

    public async Task<CheckScamResponse> ProcessCheckScamAsync(CheckScamRequest request, CancellationToken cancellationToken = default)
    {
        var transactions = _statementService.ExtractTransactions(request.RawText);
        
        var response = new CheckScamResponse
        {
            Transactions = transactions
        };

        if (!transactions.Any())
        {
            response.ScamAnalysis = new ScamDetectionResult
            {
                IsScam = false,
                Reason = "No valid transactions found in the raw text."
            };
            return response;
        }

        // Tái tạo lại chuỗi text được chuẩn hoá báo cáo AI
        var formattedTransactions = string.Join("\n", transactions.Select((t, i) => $"[{i+1}]: {t.RawContent}"));

        try
        {
            var scamAnalysis = await _geminiClient.AnalyzeScamRiskAsync(formattedTransactions, cancellationToken);
            response.ScamAnalysis = scamAnalysis;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gemini Risk Analysis Failed");
            response.ScamAnalysis = new ScamDetectionResult
            {
                IsScam = false,
                Reason = "Failed to connect to AI system for risk analysis."
            };
        }

        return response;
    }
}
