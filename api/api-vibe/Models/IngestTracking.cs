using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_vibe.Models
{
    [Table("IngestTracking")]
    public class IngestTracking
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string RawContent { get; set; }

        [Required]
        [MaxLength(50)]
        public string State { get; set; } = "idle"; // idle, parsing, parsed, error

        [MaxLength(1000)]
        public string FailureReason { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
