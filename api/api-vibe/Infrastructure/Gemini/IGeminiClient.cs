namespace api_vibe.Infrastructure.Gemini;

public interface IGeminiClient
{
    Task<string> FetchCurrentGasPriceAsync(CancellationToken cancellationToken = default);
}
