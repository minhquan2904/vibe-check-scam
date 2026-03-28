using api_vibe.Models.Requests;
using api_vibe.Models.Responses;
using api_vibe.Services;
using Microsoft.AspNetCore.Mvc;

namespace api_vibe.Controllers;

[ApiController]
[Route("api/v1/check-scam")]
public class CheckScamController : ControllerBase
{
    private readonly ICheckScamService _checkScamService;

    public CheckScamController(ICheckScamService checkScamService)
    {
        _checkScamService = checkScamService;
    }

    [HttpPost]
    public async Task<ActionResult<CheckScamResponse>> ScanRawStatement([FromBody] CheckScamRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RawText))
            return BadRequest("Raw text cannot be empty.");

        var result = await _checkScamService.ProcessCheckScamAsync(request, cancellationToken);
        return Ok(result);
    }
}
