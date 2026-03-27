using api_vibe.HealthCheck.Models;
using Microsoft.Extensions.Options;

using System.Diagnostics;

namespace api_vibe.HealthCheck.Checks;

/// <summary>
/// Database health check — executes "SELECT 1" via an injected IDbConnection factory.
/// Register your IDbConnection factory in DI and inject here, or replace this stub
/// with a typed DbContext dependency once the project has a database configured.
/// </summary>
public class DatabaseHealthCheck(
    IOptions<HealthCheckOptions> options,
    ILogger<DatabaseHealthCheck> logger,
    IDbConnectionFactory? connectionFactory = null) : IDependencyHealthCheck
{
    public string Name => "database";

    public async Task<HealthEntry> PingAsync(CancellationToken ct)
    {
        if (connectionFactory is null)
        {
            // No DB configured yet — return healthy stub
            return new HealthEntry { Status = "UP", ResponseTimeMs = 0 };
        }

        var sw = Stopwatch.StartNew();
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(options.Value.DependencyTimeoutMs);

        try
        {
            await using var conn = connectionFactory.CreateConnection();
            await conn.OpenAsync(cts.Token);
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT 1";
            await cmd.ExecuteScalarAsync(cts.Token);

            sw.Stop();
            return new HealthEntry { Status = "UP", ResponseTimeMs = sw.ElapsedMilliseconds };
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            logger.LogWarning("Database health check timed out after {TimeoutMs}ms", options.Value.DependencyTimeoutMs);
            return new HealthEntry { Status = "TIMEOUT", ResponseTimeMs = sw.ElapsedMilliseconds };
        }
        catch (Exception ex)
        {
            sw.Stop();
            logger.LogError(ex, "Database health check failed");
            return new HealthEntry { Status = "DOWN", ResponseTimeMs = sw.ElapsedMilliseconds };
        }
    }
}

