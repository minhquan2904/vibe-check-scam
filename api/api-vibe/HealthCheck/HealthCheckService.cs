using api_vibe.HealthCheck.Models;
using Microsoft.Extensions.Options;

namespace api_vibe.HealthCheck;

public class HealthCheckService(
    IEnumerable<IDependencyHealthCheck> checks,
    IOptions<HealthCheckOptions> options,
    ILogger<HealthCheckService> logger) : IHealthCheckService
{
    public async Task<HealthReadinessResponse> CheckAllAsync()
    {
        using var cts = new CancellationTokenSource(options.Value.DependencyTimeoutMs + 100);
        var results = new Dictionary<string, HealthEntry>();

        foreach (var check in checks)
        {
            try
            {
                var entry = await check.PingAsync(cts.Token);
                results[check.Name] = entry;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception in dependency check: {Name}", check.Name);
                results[check.Name] = new HealthEntry { Status = "DOWN", ResponseTimeMs = -1 };
            }
        }

        var overallStatus = ComputeOverallStatus(results);
        return new HealthReadinessResponse
        {
            Status = overallStatus,
            Timestamp = DateTimeOffset.UtcNow.ToString("o"),
            Version = GetVersion(),
            Uptime = GetUptimeSeconds(),
            Dependencies = results
        };
    }

    private static string ComputeOverallStatus(Dictionary<string, HealthEntry> results)
    {
        // DOWN: database (critical) is DOWN
        if (results.TryGetValue("database", out var db) &&
            db.Status is "DOWN" or "TIMEOUT")
        {
            return "DOWN";
        }

        // DEGRADED: any non-critical dep is DOWN/TIMEOUT
        if (results.Any(r => r.Value.Status is "DOWN" or "TIMEOUT"))
        {
            return "DEGRADED";
        }

        return "UP";
    }

    private static string GetVersion() =>
        Environment.GetEnvironmentVariable("APP_VERSION")
        ?? typeof(HealthCheckService).Assembly.GetName().Version?.ToString()
        ?? "unknown";

    internal static readonly System.Diagnostics.Stopwatch _startTime = System.Diagnostics.Stopwatch.StartNew();

    private static long GetUptimeSeconds() => (long)_startTime.Elapsed.TotalSeconds;
}
