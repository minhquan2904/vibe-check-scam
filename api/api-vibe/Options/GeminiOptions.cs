namespace api_vibe.Options;

public class GeminiOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string Model { get; set; } = "gemini-1.5-flash";
}

public class GasPriceOptions
{
    public int CacheDurationMinutes { get; set; } = 60;
}
