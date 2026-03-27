namespace api_vibe.Services;

public interface IGasPriceService
{
    Task<string> GetCurrentGasPriceAsync(CancellationToken cancellationToken = default);
}
