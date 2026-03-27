using System.ComponentModel.DataAnnotations;

namespace api_vibe.DTOs
{
    public class TransactionIngestRequest
    {
        [Required]
        [MinLength(5)]
        public string Content { get; set; }
    }
}
