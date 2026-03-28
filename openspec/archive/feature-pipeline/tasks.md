## 1. DTO Models (Backend)

- [x] 1.1 Khởi tạo `CheckScamRequest`: File `Requests/CheckScamRequest.cs` chứa thuộc tính `RawText` (string).
- [x] 1.2 Khởi tạo DTO `TransactionItem` lưu thông tin từng giao dịch đã trích xuất: `Models/TransactionItem.cs`.
- [x] 1.3 Khởi tạo DTO `ScamDetectionResult` chứa kết quả AI (`IsScam`, `ConfidenceScore`, `Reason`): `Models/ScamDetectionResult.cs`.
- [x] 1.4 Khởi tạo `CheckScamResponse` wrap dữ liệu trả về cho API đầu cuối: `Responses/CheckScamResponse.cs`.

## 2. API Options & Core Interfaces (Backend)

- [x] 2.1 Tạo cấu hình `GeminiOptions.cs` trong `Options/GeminiOptions.cs` (đã có sẵn ở dự án, có thể review lại nếu cần thiết bổ sung field).
- [x] 2.2 Định nghĩa `IStatementProcessingService.cs` trong `Services/Interfaces/`. Chứa hàm `ExtractTransactions(string rawText)`.
- [x] 2.3 Định nghĩa `IGeminiDetectionService.cs` (hoặc mở rộng `IGeminiClient`) trong `Services/Interfaces/`. Chứa hàm `AnalyzeRiskAsync(List<TransactionItem> items)`.
- [x] 2.4 Định nghĩa `ICheckScamService.cs` điều phối luồng chính.

## 3. Services Implementation (Backend)

- [x] 3.1 Triển khai `StatementProcessingService.cs` (tách dòng, xóa ký tự rác, chuẩn hóa raw text thành list).
- [x] 3.2 Cập nhật `GeminiClient.cs`: thêm hàm gọi endpoint Gemini với prompt dành riêng cho check scam và parse JSON response.
- [x] 3.3 Triển khai `CheckScamService.cs`: nối module `Statement` và `Gemini` theo chain, mapping kết quả ra `CheckScamResponse`.
- [x] 3.4 Cấu hình file `appsettings.Development.json` và đăng ký DI container ở `Program.cs`.

## 4. Controller (Backend)

- [x] 4.1 Khởi tạo `CheckScamController.cs` trong `Controllers/`. Route `[Route("api/v1/check-scam")]`, method `[HttpPost]`, nhận request gửi vào `ICheckScamService` rồi trả response.
