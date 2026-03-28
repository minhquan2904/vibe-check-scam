## Context

Tính năng `check-scam` đóng vai trò trung gian xử lý, tự động phân tích và bóc tách các đoạn thông tin giao dịch từ một chuỗi (raw text) văn bản sao kê do người dùng đưa vào. Mục tiêu dài hạn là ứng dụng trí tuệ nhân tạo (Gemini API) nhằm phân loại, đánh giá rủi ro và nhận dạng các dấu hiệu lừa đảo (scam detection) một cách tự động, từ đó bảo vệ người dùng và tránh các tổn thất về tài chính.

## Goals / Non-Goals

**Goals:**
- Cung cấp HTTP Endpoint `/api/v1/check-scam` xử lý raw_text từ hệ thống client.
- Xây dựng `StatementProcessingService` chịu trách nhiệm chuẩn hóa văn bản.
- Xây dựng `CheckScamService` để gửi yêu cầu phân tích thông qua Gemini API. Cung cấp kết quả đánh giá (bình thường, cảnh báo) vào response.

**Non-Goals:**
- Phát triển tính năng trích xuất văn bản từ hình ảnh/OCR (Optical Character Recognition).
- Tạo nghiệp vụ khóa tải khoản (block account), chặn chuyển tiền thủ công - logic này thuộc hệ thống rủi ro khác.

## Decisions

1. **Giao tiếp API với Gemini:** Sử dụng SDK hoặc cơ chế HTTP Client với Rest call tới endpoint API của Gemini (Google). Api key được cấu hình thông qua `appsettings.json` với pattern bảo mật chung của dự án.
2. **Loại bỏ dữ liệu PII (Data Anonymization):** Thiết kế pipeline phải cho phép bổ sung bộ lọc mask các thông tin nhạy cảm của khách hàng (PII) như tài khoản định danh, số CMND... trước khi đưa lên xử lý ở Third-party API.
3. **Phân tách trách nhiệm:** Tách biệt module lấy dữ liệu (API Controller) và module xử lý ngữ cảnh (Service/Pipeline Flow) để dễ dàng thay đổi nhà cung cấp AI sau này.

## Risks / Trade-offs

- **[Risk] Giới hạn Rate Limit & Timeout của Gemini API**: LLM mất thời gian suy luận dài gây timeout request từ hệ thống chính. → *Mitigation*: Cài đặt custom HttpClient timeout, cấu hình fallback response khi hệ thống AI không kịp phản hồi tránh làm treo luồng.
- **[Risk] Rủi ro Halucination (AI nói dối)**: AI có thể quy chụp nhầm (False Positive). → *Mitigation*: Chỉnh sửa system prompt cẩn thận, yêu cầu format trả ra JSON cứng có field `IsScam`, `ConfidenceScore`, `Reason` để dễ dàng parsing trên C#.
