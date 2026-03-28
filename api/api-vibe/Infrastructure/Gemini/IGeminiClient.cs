using api_vibe.Models;

namespace api_vibe.Infrastructure.Gemini;

public interface IGeminiClient
{
    Task<string> FetchCurrentGasPriceAsync(CancellationToken cancellationToken = default);
    Task<ScamDetectionResult> AnalyzeScamRiskAsync(string statementText, CancellationToken cancellationToken = default);
}
