# New API Endpoints

| # | Method | Path | Controller | Handler | Description |
|---|--------|------|------------|---------|-------------|
| 1 | POST | `/api/v1/check-scam` | `CheckScamController` | `ScanRawStatement` | Tiếp nhận raw text sao kê, bóc tách giao dịch và gọi Gemini AI để phân tích dấu hiệu lừa đảo. |

## Endpoint Details: `/api/v1/check-scam`

- **Purpose**: Phân tích rủi ro lừa đảo (scam detection) từ nội dung sao kê thô.
- **Authentication**: `[ApiController]` (Chưa có `[Authorize]` cụ thể theo code hiện tại, follow base controller nếu có).
- **Request Body** (`CheckScamRequest`):
  ```json
  {
      "rawText": "nội dung sao kê..."
  }
  ```
- **Response Body** (`CheckScamResponse`):
  ```json
  {
      "transactions": [
          { "rawContent": "giao dịch 1" }
      ],
      "scamAnalysis": {
          "isScam": true,
          "confidenceScore": 0.95,
          "reason": "Mô tả lý do..."
      }
  }
  ```
