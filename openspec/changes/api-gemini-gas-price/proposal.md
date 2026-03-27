## Why

Cần cung cấp một API tự động truy vấn và phân tích thông tin giá xăng hiện tại trên thị trường thông qua tích hợp với AI service (Gemini). Điều này cung cấp dữ liệu tức thì, liên tục được cập nhật bằng LLM scraping hoặc direct prompt, hỗ trợ cho các tính năng người dùng cuối tra cứu thông tin nhanh chóng.

## What Changes

- Bổ sung integration client (gọi Gemini API) vào project backend.
- Tạo controller endpoint mới `GET /api/v1/gas-price/today`.
- Logic nhận diện, fetch giá xăng và parse thành cấu trúc JSON trả về cho clients.

## Capabilities

### New Capabilities
- `gas-price-check`: Chức năng gọi Gemini API để phân tích và trả về thông tin giá xăng hôm nay.

### Modified Capabilities
- 

## Impact

- **Affected Code**: AppSettings (cần config API Key Gemini), Service Layer để call Http/AI client.
- **Dependencies**: Có thể cần thêm thư viện gọi HTTP Request hoặc SDK cho Gemini (nếu chưa có).
- **System**: Cần đảm bảo handle retry / timeout khi kết nối với LLM service.
