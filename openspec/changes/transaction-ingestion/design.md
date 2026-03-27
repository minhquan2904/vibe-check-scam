## Context
Hệ thống cần tự động hóa việc ingest (tiếp nhận) các sao kê ngân hàng (VCB, BIDV, MB Bank) dưới dạng text thô hoặc CSV thông qua sự trợ giúp của AI LLM thay vì yêu cầu người dùng nhập liệu từng field thủ công.

## Goals / Non-Goals
**Goals:**
- Thiết kế API hỗ trợ nhận cấu trúc dữ liệu thô (raw text).
- Đảm bảo luồng trạng thái rõ ràng (idle → parsing → parsed / error) do quá trình gọi AI có thể mất thời gian.

**Non-Goals:**
- Không xử lý parse File dạng PDF hoặc hình ảnh (OCR) trong Phase này.

## Feature Profile
- **Mode:** Command
- **Flow Type:** NonFinancial / Data Ingestion
- **Auth:** REQUIRED
- **Layers:** Controller → Service (TransactionIngestionService, AICoreService) → Repository

## API Contracts
### `POST /api/v1/transactions/ingest`
- **Auth:** REQUIRED
- **Request:** `TransactionIngestRequest` (property: `Content` - string)
- **Response:** `BaseResponse<IngestTrackingResponse>` (chứa `TrackingId`, `State`)

### `GET /api/v1/transactions/ingest/{trackingId}`
- **Auth:** REQUIRED
- **Response:** `BaseResponse<IngestTrackingResponse>` (chứa tiến độ, trạng thái `parsing`, `parsed`, hoặc danh sách data đã parse).

## Component Design
1. **TransactionIngestController**: Xử lý HTTP Request, Authorize, gọi Service layer.
2. **TransactionIngestionService**: 
   - Khởi tạo tracking lưu DB với state `idle`.
   - Update state sang `parsing` và giao cho Background Worker hoặc gọi trực tiếp AI client.
3. **AICoreService**: Đóng gói logic gọi Gemini API, truyền prompt và raw text để lấy JSON có cấu trúc.
4. Chuyển JSON thành List Entities và insert vào Database, cập nhật Tracking state sang `parsed`.

## Entity Design
**Table: IngestTracking**
- `Id`: Guid / String (Primary Key)
- `RawContent`: Text (Dữ liệu gốc gửi lên)
- `State`: Varchar (`idle`, `parsing`, `parsed`, `error`)
- `FailureReason`: Varchar (Lưu lỗi nếu có)
- `CreatedAt`, `UpdatedAt`: DateTime

**Table: Transaction** (Bảng chính chứa dữ liệu giao dịch - liên kết TrackingId)
- `AccountNumber`, `Amount`, `Description`, `TransactionDate`, `BankCode`, `IngestTrackingId`

## Configuration
- `Gemini:ApiKey`: Secret key (lưu trong user secrets hoặc AppSettings).
- `Gemini:IngestPrompt`: Template hướng dẫn lấy data từ raw text.
- `Ingest:MaxRetry`: Cấu hình số lần thử lại nếu gọi AI fail.

## Error Codes
- `400` | `ERR_INGEST_01` | Dữ liệu đầu vào không hợp lệ hoặc rỗng.
- `503` | `ERR_INGEST_02` | Dịch vụ AI phân tích hiện không phản hồi.
