## Why

Hệ thống cần cung cấp khả năng phân tích danh sách sao kê ngân hàng dạng raw text để tự động phân loại giao dịch và phát hiện các dấu hiệu lừa đảo (scam). Việc phân tích này dựa vào Gemini AI nhằm tự động hóa và tăng độ chính xác trong việc phát hiện bất thường, từ đó bảo vệ hệ thống và người dùng.

## What Changes

- Xây dựng workflow để nhận chuỗi text thô (raw text) chứa nội dung sao kê.
- Xây dựng logic phân loại các giao dịch.
- Tích hợp Gemini API để kiểm tra các giao dịch này xem có dấu hiệu lừa đảo hay không.
- Trả về kết quả phân tích và đánh giá rủi ro cho client.

## Capabilities

### New Capabilities
- `statement-text-processing`: Tiếp nhận và trích xuất dữ liệu từ raw text sao kê ngân hàng.
- `gemini-scam-detection`: Gọi API Gemini để phân tích cảnh báo lừa đảo.

### Modified Capabilities


## Impact

- **API:** Mở endpoint mới (ví dụ `/api/v1/feature-pipeline`) để client có thể gửi raw text.
- **Dependencies:** Bổ sung SDK hoặc cơ chế HTTP Client gọi đến AI (Gemini REST API). Cần thêm thông tin cấu hình API keys trong `appsettings.json`.
