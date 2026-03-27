namespace api_vibe.HealthCheck;

public class HealthCheckOptions
{
    public int DependencyTimeoutMs { get; set; } = 500;
    public string[] ExcludeAuthRoutes { get; set; } = ["/health", "/health/ready"];
}
