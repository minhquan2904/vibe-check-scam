namespace api_vibe.Models.Requests;

public class CheckScamRequest
{
    /// <summary>
    /// Nội dung sao kê dưới dạng text nguyên bản.
    /// </summary>
    public string RawText { get; set; } = string.Empty;
}
