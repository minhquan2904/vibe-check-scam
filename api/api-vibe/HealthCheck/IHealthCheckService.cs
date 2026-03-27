using api_vibe.HealthCheck.Models;

namespace api_vibe.HealthCheck;

public interface IHealthCheckService
{
    Task<HealthReadinessResponse> CheckAllAsync();
}
