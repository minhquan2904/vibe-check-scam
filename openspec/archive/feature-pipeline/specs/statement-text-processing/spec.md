## ADDED Requirements

### Requirement: Tiệp nhận và trích xuất raw text sao kê
Hệ thống SHALL nhận đầu vào là chuỗi text văn bản thô do người dùng upload hoặc nhập, đại diện cho nội dung sao kê ngân hàng, sau đó tiến hành phân tách và trích xuất sơ bộ các đoạn văn bản chứa giao dịch.

#### Scenario: Nhận text hợp lệ
- **WHEN** một request chứa raw text hợp lệ được gửi lên
- **THEN** hệ thống xử lý nội dung, loại bỏ ký tự rác và chuẩn bị dữ liệu văn bản sạch để phân tích tiếp

#### Scenario: Nhận text rỗng
- **WHEN** hệ thống nhận raw text rỗng hoặc không có bất kỳ ký tự nào
- **THEN** hệ thống ném ra ngoại lệ hoặc lỗi Validation
