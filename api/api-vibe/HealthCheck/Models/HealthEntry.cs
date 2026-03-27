namespace api_vibe.HealthCheck.Models;

public class HealthEntry
{
    public string Status { get; init; } = "UNKNOWN";
    public long ResponseTimeMs { get; init; }
}
