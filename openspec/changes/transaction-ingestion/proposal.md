## Why

Hệ thống cần có khả năng tự động xử lý các đoạn text sao kê copy/paste hoặc file CSV từ các định dạng ngân hàng phổ biến (Vietcombank, BIDV, MB Bank) nhằm tiết kiệm thời gian nhập liệu thủ công, giảm sai sót và tự động hóa luồng kiểm tra thông tin lừa đảo.

## What Changes

- Xây dựng API (Backend) và UI (Frontend) cho phép nhập chuỗi text chứa nội dung sao kê thô hoặc upload file CSV.
- Tích hợp AI (Gemini) để parse dữ liệu text thô (raw text) thành dữ liệu có cấu trúc (gồm: số tài khoản, số tiền, nội dung chuyển khoản, ngày giờ giao dịch, ngân hàng gửi/nhận...).
- Bổ sung luồng quản lý trạng thái xử lý dữ liệu: `idle` → `parsing` → `parsed` hoặc `error`.

## Capabilities

### New Capabilities

- `transaction-parsing`: Khả năng tiếp nhận raw text/csv và chuyển đổi thành giao dịch có cấu trúc thông qua AI parsing, kèm quản lý tiến trình (`idle`, `parsing`, `parsed`, `error`).

### Modified Capabilities



## Impact

- **Database**: Cần có table lưu trữ các giao dịch thô (raw transaction) và trạng thái parsing, kết quả parse thành công.
- **Backend (api-vibe)**: Thêm các API endpoint tiếp nhận data (ingest data), service gọi LLM để trích xuất thông tin.
- **Frontend (Web)**: Thêm giao diện textbox lớn cho việc dán text (paste) hoặc component upload CSV, hiển thị trạng thái xử lý tương ứng.
