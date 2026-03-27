using System;
using System.Threading.Tasks;
using api_vibe.DTOs;
using api_vibe.Services;

namespace api_vibe.Services
{
    public class TransactionIngestionService : ITransactionIngestionService
    {
        private readonly IAICoreService _aiCoreService;

        public TransactionIngestionService(IAICoreService aiCoreService)
        {
            _aiCoreService = aiCoreService;
        }

        public async Task<IngestTrackingResponse> IngestAsync(TransactionIngestRequest request)
        {
            // Simulate saving tracking record to DB with idle state
            var trackingId = Guid.NewGuid();

            // Fire and forget parsing logic (or enqueue to a background worker)
            _ = Task.Run(async () =>
            {
                try
                {
                    // State becomes parsing
                    string parsedJson = await _aiCoreService.ParseTransactionAsync(request.Content);
                    // Map parsedJson to entity and save, update state to parsed
                }
                catch (Exception)
                {
                    // Update state to error
                }
            });

            return new IngestTrackingResponse
            {
                TrackingId = trackingId,
                State = "idle",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };
        }

        public async Task<IngestTrackingResponse> GetStatusAsync(Guid trackingId)
        {
            // Simulate DB read
            return new IngestTrackingResponse
            {
                TrackingId = trackingId,
                State = "parsing",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };
        }
    }
}
