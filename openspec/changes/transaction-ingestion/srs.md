# Software Requirements Specification: Transaction Ingestion

## 1. Feature Scope and Business Flow
- **Mục tiêu (Goal):** Tiếp nhận dữ liệu sao kê thô dạng text hoặc file CSV từ các ngân hàng (VCB, BIDV, MB Bank), sau đó sử dụng AI để tự động phân tích và trích xuất thành dữ liệu giao dịch có cấu trúc.
- **Quy trình (Business Flow):**
  1. Người dùng gửi dữ liệu thô (raw text / CSV) lên hệ thống qua API/UI.
  2. Hệ thống tạo bản ghi theo dõi (tracking record) với trạng thái `idle`.
  3. Hệ thống đổi trạng thái thành `parsing` và gọi dịch vụ AI (Gemini) để trích xuất thông tin: số tài khoản, số tiền, ngày giờ, nội dung, mã ngân hàng.
  4. Xử lý kết quả từ AI, đối soát lưu vào dữ liệu giao dịch. Cập nhật trạng thái thành `parsed` (thành công) hoặc `error` (thất bại).

## 2. Feature Profile
- **Mode:** Command
- **Flow Type:** NonFinancial / Feature
- **Auth:** REQUIRED

## 3. Requirements (Yêu cầu chức năng)
- **REQ-01:** Hệ thống **MUST** cung cấp API để ingest dữ liệu sao kê thô (chuỗi text hoặc file).
- **REQ-02:** Hệ thống **MUST** quản lý và chuyển đổi trạng thái (state machine) của tác vụ parsing: `idle` → `parsing` → `parsed` / `error`.
- **REQ-03:** AI Service **SHALL** trích xuất ít nhất các thông tin sau từ raw text: `AccountNumber`, `Amount`, `TransactionDate`, `Description`, `BankCode`.
- **REQ-04:** Nếu việc trích xuất thất bại hoặc dữ liệu mập mờ, hệ thống **MUST** đánh dấu là `error` để người dùng (Agent/Admin) review lại thủ công.
- **REQ-05:** Frontend **SHOULD** hiển thị danh sách các lần ingest và trạng thái xử lý tương ứng.

## 4. Error Scenarios
- **ERR-01 (Invalid Input):** Dữ liệu input trống hoặc không hợp lệ -> HTTP 400 Bad Request.
- **ERR-02 (AI Parsing Failed):** AI không thể trích xuất do text rác hoặc định dạng không nhận dạng được -> State: `error`, lưu log lý do.
- **ERR-03 (Service Unavailable):** Lỗi kết nối tới AI Provider (Gemini API down) -> HTTP 503 Service Unavailable (Nếu parse đồng bộ) hoặc retry/fail (nếu chạy background).
