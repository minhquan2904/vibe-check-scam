using api_vibe.HealthCheck.Models;

namespace api_vibe.HealthCheck;

public interface IDependencyHealthCheck
{
    string Name { get; }
    Task<HealthEntry> PingAsync(CancellationToken ct);
}
