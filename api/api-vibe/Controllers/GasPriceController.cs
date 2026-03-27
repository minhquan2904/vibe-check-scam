using Microsoft.AspNetCore.Mvc;
using api_vibe.Services;
using Microsoft.AspNetCore.Authorization;

namespace api_vibe.Controllers;

[ApiController]
[Route("api/v1/gas-price")]
public class GasPriceController : ControllerBase
{
    private readonly IGasPriceService _gasPriceService;
    private readonly ILogger<GasPriceController> _logger;

    public GasPriceController(IGasPriceService gasPriceService, ILogger<GasPriceController> logger)
    {
        _gasPriceService = gasPriceService;
        _logger = logger;
    }

    [HttpGet("today")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTodayGasPrice(CancellationToken cancellationToken)
    {
        try
        {
            var jsonResult = await _gasPriceService.GetCurrentGasPriceAsync(cancellationToken);
            return Content(jsonResult, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get today gas price from Gemini");
            return StatusCode(503, new { error = "ERR_EXTERNAL_SERVICE_DOWN", message = "Third party service is unavailable" });
        }
    }
}
