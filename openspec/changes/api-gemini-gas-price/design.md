## Context

Hệ thống cần cung cấp thông tin giá xăng hiện tại cho người dùng. Thay vì scraping các trang web tĩnh dễ bị hỏng khi layout thay đổi, hệ thống sẽ gọi API của LLM (Gemini) để linh hoạt phân tích và trích xuất giá xăng hôm nay một cách tự nhiên.

## Feature Profile

- **Mode**: Custom (External API Integration)
- **Flow Type**: Query
- **Auth**: EXCLUDED (`[AllowAnonymous]`)
- **Layers**: Controller → Service → External HTTP Client

## Goals / Non-Goals

**Goals:**
- Cung cấp endpoint lấy giá xăng hôm nay một cách tự động.
- Tích hợp gọi Google Gemini API một cách an toàn.
- Caching kết quả để tránh hit rate limit của Gemini API và tăng tốc độ response.

**Non-Goals:**
- Lưu trữ lịch sử giá xăng vào database.
- Dự đoán giá xăng trong tương lai.

## API Contracts

### `GET /api/v1/gas-price/today`
- **Auth**: `[AllowAnonymous]`
- **Request**: None
- **Response** (200 OK):
  ```json
  {
    "price": 24000,
    "unit": "VND/lít",
    "ron95": 24000,
    "e5ron92": 23000,
    "diesel": 20000,
    "timestamp": "2026-03-27T12:00:00Z"
  }
  ```

## Component Design

- **GasPriceController**: Expose endpoint `GET /api/v1/gas-price/today`.
- **IGasPriceService / GasPriceService**: Chứa logic kiểm tra cache `IMemoryCache`. Nếu miss cache, gọi `IGeminiClient`.
- **IGeminiClient / GeminiClient**: Trực tiếp kết nối tới Gemini API bằng `HttpClient` (sử dụng Typed Client pattern).

## Entity Design

- Không có Entity / Không lưu database.

## Configuration

- `Gemini:ApiKey`: Chuỗi API key cấp bởi Google.
- `Gemini:Endpoint`: URL của Gemini API (e.g., `https://generativelanguage.googleapis.com/...`).
- `Gemini:Model`: Tên model (e.g., `gemini-1.5-flash`).
- `GasPrice:CacheDurationMinutes`: Thời gian lưu cache (Default: `60`).

## Decisions

- **Decision 1**: Sử dụng `IMemoryCache` lưu kết quả gọi LLM trong 60 phút.
  - *Rationale*: Giảm load, tránh bị rate limit và tiết kiệm chi phí do API AI tốn thời gian phản hồi.
- **Decision 2**: Gửi prompt yêu cầu Gemini format kết quả đầu ra dạng JSON (`response_mime_type: application/json`).
  - *Rationale*: Dễ dàng parse bằng `System.Text.Json` mà không cần RegEx regex phức tạp.

## Error Codes

- **HTTP 200**: Thành công.
- **HTTP 503 (Service Unavailable)**: Gemini API timeout hoặc down.
  - Business Code: `ERR_EXTERNAL_SERVICE_DOWN`
- **HTTP 500 (Internal Server Error)**: Lỗi parsing JSON hoặc lỗi hệ thống khác.

## Risks / Trade-offs

- **Risk**: Gemini API bị thay đổi format response hoặc trả về hallucination.
  - *Mitigation*: Bắt buộc Gemini trả về JSON schema cố định, kèm theo logic fallback validation ở service.
