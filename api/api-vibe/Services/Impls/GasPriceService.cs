using api_vibe.Infrastructure.Gemini;
using api_vibe.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace api_vibe.Services.Impls;

public class GasPriceService : IGasPriceService
{
    private readonly IGeminiClient _geminiClient;
    private readonly IMemoryCache _cache;
    private readonly GasPriceOptions _options;
    private readonly ILogger<GasPriceService> _logger;
    private const string CacheKey = "GasPrice_Today";

    public GasPriceService(
        IGeminiClient geminiClient, 
        IMemoryCache cache, 
        IOptions<GasPriceOptions> options, 
        ILogger<GasPriceService> logger)
    {
        _geminiClient = geminiClient;
        _cache = cache;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GetCurrentGasPriceAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKey, out string? cachedPrice) && !string.IsNullOrEmpty(cachedPrice))
        {
            _logger.LogInformation("Gas price retrieved from cache");
            return cachedPrice;
        }

        _logger.LogInformation("Gas price cache miss. Fetching from external LLM service.");
        var newPriceJson = await _geminiClient.FetchCurrentGasPriceAsync(cancellationToken);

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(_options.CacheDurationMinutes));

        _cache.Set(CacheKey, newPriceJson, cacheEntryOptions);

        return newPriceJson;
    }
}
