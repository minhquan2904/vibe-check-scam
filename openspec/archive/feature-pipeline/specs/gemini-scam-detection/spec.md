## ADDED Requirements

### Requirement: Gọi Gemini API để phân loại và đánh giá rủi ro lừa đảo
Hệ thống SHALL tổng hợp dữ liệu text sao kê và gọi tới Gemini API (với prompt chuyên dụng) nhằm phân loại các loại giao dịch và phát hiện các dấu hiệu lừa đảo (scam) có thể tồn tại.

#### Scenario: Phân tích thành công danh sách giao dịch an toàn
- **WHEN** lời gọi Gemini API trả về phân loại giao dịch bình thường, không có dấu hiệu scam
- **THEN** hệ thống ánh xạ kết quả này thành response an toàn và trả về cho client

#### Scenario: Khám phá giao dịch có dấu hiệu lừa đảo (scam)
- **WHEN** lời gọi Gemini API phân loại một đoạn text chứa dấu hiệu lừa đảo (ví dụ: chuyển tiền bất thường, tài khoản bị blacklist, nội dung khả nghi)
- **THEN** hệ thống nhận kết quả, đánh dấu transaction bị cảnh báo cùng mô tả chi tiết từ AI và trả về cho client

#### Scenario: Xử lý lỗi kết nối AI
- **WHEN** việc gọi Gemini API gặp sự cố (timeout, 5xx, hoặc giới hạn rate limit)
- **THEN** hệ thống ghi nhận lỗi vào log và trả về HTTP response biểu thị lỗi Third-party service (Gemini)
