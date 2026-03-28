# Delta Specification

## Impact Scope
| Component | Before | After | Impact |
|-----------|--------|-------|--------|
| GeminiClient | Chỉ có hàm lấy giá xăng | Thêm hàm AnalyzeScamRiskAsync gọi Gemini AI | Extension |
| Services | Chưa có logic xử lý sao kê | Thêm StatementProcessingService & CheckScamService | New Module |
| Controllers | Chưa có endpoint xử lý scam | Thêm CheckScamController | New API |

## Behavioral Changes
| # | Area | Previous | Current | Type |
|---|------|----------|---------|------|
| 1 | Scam Detection | N/A | Hệ thống có khả năng nhận diện rủi ro lừa đảo từ raw text sao kê qua AI. | ADDED |
| 2 | DI Container | Chỉ đăng ký GasPrice services | Đăng ký thêm Statement & CheckScam services | MODIFIED |
