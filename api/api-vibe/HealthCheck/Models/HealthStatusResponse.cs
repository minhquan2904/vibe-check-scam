namespace api_vibe.HealthCheck.Models;

public class HealthStatusResponse
{
    public string Status { get; init; } = "UP";
    public string Timestamp { get; init; } = DateTimeOffset.UtcNow.ToString("o");
    public string Version { get; init; } = "1.0.0";
    public long Uptime { get; init; }
}
