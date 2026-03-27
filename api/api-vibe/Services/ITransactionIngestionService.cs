using System;
using System.Threading.Tasks;
using api_vibe.DTOs;

namespace api_vibe.Services
{
    public interface ITransactionIngestionService
    {
        Task<IngestTrackingResponse> IngestAsync(TransactionIngestRequest request);
        Task<IngestTrackingResponse> GetStatusAsync(Guid trackingId);
    }
}
