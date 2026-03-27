using api_vibe.HealthCheck;
using api_vibe.HealthCheck.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_vibe.Controllers;

[ApiController]
[Route("health")]
[AllowAnonymous]
public class HealthCheckController(
    IHealthCheckService healthCheckService,
    ILogger<HealthCheckController> logger) : ControllerBase
{
    private static string GetVersion() =>
        Environment.GetEnvironmentVariable("APP_VERSION")
        ?? typeof(HealthCheckController).Assembly.GetName().Version?.ToString()
        ?? "unknown";

    private static long GetUptimeSeconds() =>
        (long)api_vibe.HealthCheck.HealthCheckService._startTime.Elapsed.TotalSeconds;

    [HttpGet("")]
    [ProducesResponseType(typeof(HealthStatusResponse), StatusCodes.Status200OK)]
    public IActionResult Liveness()
    {
        var response = new HealthStatusResponse
        {
            Status = "UP",
            Timestamp = DateTimeOffset.UtcNow.ToString("o"),
            Version = GetVersion(),
            Uptime = GetUptimeSeconds()
        };
        return Ok(response);
    }

    [HttpGet("ready")]
    [ProducesResponseType(typeof(HealthReadinessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthReadinessResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Readiness()
    {
        try
        {
            var result = await healthCheckService.CheckAllAsync();
            return result.Status == "DOWN"
                ? StatusCode(StatusCodes.Status503ServiceUnavailable, result)
                : Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception in health readiness check");
            var errorResponse = new HealthReadinessResponse
            {
                Status = "DOWN",
                Timestamp = DateTimeOffset.UtcNow.ToString("o"),
                Version = GetVersion(),
                Uptime = GetUptimeSeconds()
            };
            return StatusCode(StatusCodes.Status503ServiceUnavailable, errorResponse);
        }
    }
}
