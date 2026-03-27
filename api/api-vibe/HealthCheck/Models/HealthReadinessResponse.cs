namespace api_vibe.HealthCheck.Models;

public class HealthReadinessResponse : HealthStatusResponse
{
    public Dictionary<string, HealthEntry> Dependencies { get; init; } = [];
}
