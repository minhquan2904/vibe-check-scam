using System.Text;
using System.Text.Json;
using api_vibe.Models;
using api_vibe.Options;
using Microsoft.Extensions.Options;

namespace api_vibe.Infrastructure.Gemini;

public class GeminiClient : IGeminiClient
{
    private readonly HttpClient _httpClient;
    private readonly GeminiOptions _options;
    private readonly ILogger<GeminiClient> _logger;

    public GeminiClient(HttpClient httpClient, IOptions<GeminiOptions> options, ILogger<GeminiClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> FetchCurrentGasPriceAsync(CancellationToken cancellationToken = default)
    {
        var apiKey = _options.ApiKey;
        var model = _options.Model;
        var endpoint = $"{_options.Endpoint}/v1beta/models/{model}:generateContent?key={apiKey}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = "Lấy giá xăng hôm nay tại Việt Nam (RON 95-V, E5 RON 92, Diesel). Trả về đúng định dạng JSON sau, không markdown, không text dư thừa: {\"price\": 24000, \"unit\": \"VND/lít\", \"ron95\": 24000, \"e5ron92\": 23000, \"diesel\": 20000, \"timestamp\": \"2024-03-27T12:00:00Z\"}"
                        }
                    }
                }
            },
            generationConfig = new
            {
                responseMimeType = "application/json"
            }
        };

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var content = new StringContent(JsonSerializer.Serialize(requestBody, jsonOptions), Encoding.UTF8, "application/json");

        try
        {
            _logger.LogInformation("Attempting Gemini API. Endpoint Options: {E}, Model: {M}. Full Target (Masked): {URL}", 
                _options.Endpoint, _options.Model, endpoint.Replace(apiKey, "***"));
                
            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Gemini API Error. Status: {Status}, Body: {Body}", response.StatusCode, errorBody);
            }
            
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            using var document = JsonDocument.Parse(responseJson);

            // Extract the text part from Gemini response: candidates[0].content.parts[0].text
            var text = document.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text").GetString();

            return text ?? "{}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch gas price from Gemini API");
            throw;
        }
    }

    public async Task<ScamDetectionResult> AnalyzeScamRiskAsync(string statementText, CancellationToken cancellationToken = default)
    {
        var apiKey = _options.ApiKey;
        var model = _options.Model;
        var endpoint = $"{_options.Endpoint}/v1beta/models/{model}:generateContent?key={apiKey}";

        var prompt = $"Phân tích danh sách giao dịch ngân hàng sau đây và cho biết có nhận thấy dấu hiệu lừa đảo (scam) nào không. Trả về đúng định dạng JSON có cấu trúc {{ \"isScam\": boolean, \"confidenceScore\": number(0-1), \"reason\": string(mô tả chi tiết nếu có lừa đảo) }}. Không sinh thêm markdown hay text nào khác. Danh sách giao dịch:\n{statementText}";

        var requestBody = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
            generationConfig = new { responseMimeType = "application/json" }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Gemini API Error (ScamDetection). Status: {Status}, Body: {Body}", response.StatusCode, errorBody);
            }
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            using var document = JsonDocument.Parse(responseJson);
            var text = document.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

            if (string.IsNullOrWhiteSpace(text)) return new ScamDetectionResult { IsScam = false };

            var result = JsonSerializer.Deserialize<ScamDetectionResult>(text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new ScamDetectionResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze scam risk");
            throw;
        }
    }
}
