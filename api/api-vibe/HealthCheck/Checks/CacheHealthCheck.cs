using api_vibe.HealthCheck.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace api_vibe.HealthCheck.Checks;

public class CacheHealthCheck(IMemoryCache cache) : IDependencyHealthCheck
{
    private static readonly object _sentinelKey = new();

    public string Name => "cache";

    public Task<HealthEntry> PingAsync(CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            cache.TryGetValue(_sentinelKey, out _);
            sw.Stop();
            return Task.FromResult(new HealthEntry
            {
                Status = "UP",
                ResponseTimeMs = sw.ElapsedMilliseconds
            });
        }
        catch
        {
            sw.Stop();
            return Task.FromResult(new HealthEntry
            {
                Status = "DOWN",
                ResponseTimeMs = sw.ElapsedMilliseconds
            });
        }
    }
}
