# TODO & Uncover Items

## Statistics
| Type | Count |
|------|-------|
| TODO | 0 |
| FIXME | 0 |
| Edge Cases | 1 |
| Env Issue | 1 |

## Details
| # | Type | File | Line | Description |
|---|------|------|------|-------------|
| 1 | Env Issue | api-vibe.csproj | - | Project targets .NET 9.0 but current environment only has .NET 8.0 SDK. Build failed. |
| 2 | Edge Case | StatementProcessingService.cs | 12 | Logic bóc tách giao dịch hiện tại chỉ dựa trên split dòng đơn giản. Cần Regex chuẩn hóa cho đa dạng định dạng sao kê ngân hàng. |
| 3 | AI Hallucination | GeminiClient.cs | 89 | Gemini AI có thể trả về JSON không đúng cấu trúc nếu prompt bị hiểu nhầm (hallucination). Cần thêm Validation schema cho JSON response. |
