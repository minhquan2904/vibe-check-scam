using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api_vibe.DTOs;
using api_vibe.Services;

namespace api_vibe.Controllers
{
    [ApiController]
    [Route("api/v1/transactions/[controller]")]
    // [Authorize] -- Uncomment when auth is fully setup
    public class TransactionIngestController : ControllerBase
    {
        private readonly ITransactionIngestionService _ingestService;

        public TransactionIngestController(ITransactionIngestionService ingestService)
        {
            _ingestService = ingestService;
        }

        [HttpPost]
        public async Task<IActionResult> Ingest([FromBody] TransactionIngestRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("ERR_INGEST_01: Dữ liệu đầu vào không hợp lệ hoặc rỗng.");
            }

            var result = await _ingestService.IngestAsync(request);
            return Ok(new { Data = result });
        }

        [HttpGet("{trackingId}")]
        public async Task<IActionResult> GetStatus([FromRoute] Guid trackingId)
        {
            var result = await _ingestService.GetStatusAsync(trackingId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(new { Data = result });
        }
    }
}
