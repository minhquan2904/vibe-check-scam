using System;

namespace api_vibe.DTOs
{
    public class IngestTrackingResponse
    {
        public Guid TrackingId { get; set; }
        public string State { get; set; }
        public string FailureReason { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
