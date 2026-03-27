# Project Structure: API & Web Separation

Dự án được thiết kế theo mô hình client-server (Frontend/Backend tách biệt) nhằm đảo bảo tính độc lập, dễ dàng mở rộng và bảo trì. Cấu trúc tổng quan của hệ thống chia thành hai phần chính:

## 1. Hệ thống Backend (`/api/`)
Thư mục `api/` (hiện tại chứa project `api-vibe`) là hệ thống Backend được phát triển bằng **.NET (C#)** – cụ thể là ASP.NET Core Web API.
- **Nhiệm vụ:** Xử lý logic nghiệp vụ, bảo mật, thao tác với cơ sở dữ liệu và cung cấp các RESTful APIs cho phía Frontend (Web/App) giao tiếp.
- **Thành phần chính:**
  - `Controllers/`: Tầng giao tiếp, định nghĩa các API Endpoints.
  - `Services/`: Tầng xử lý logic nghiệp vụ trung tâm.
  - `Infrastructure/`: Tầng kỹ thuật thao tác hạ tầng (Database, External APIs, Message Queue, Caching).
  - Định dạng mã và kiểm tra tuân thủ kiến trúc đã được định nghĩa tại `base_knowledge/common_rules/PRJ-03-dotnet-scan-rule.md`.

## 2. Hệ thống Frontend (`/web/`)
Thư mục `web/` là hệ thống Frontend (thường là **Angular** theo như cấu hình tại `PRJ-04-angular-scan-rule.md`).
- **Nhiệm vụ:** Hiển thị giao diện người dùng (UI), điều hướng (Routing), quản lý trạng thái (State Management) ở phía client, và gọi các API từ Backend để lấy/cập nhật dữ liệu.
- **Đặc điểm:** Tách biệt hoàn toàn với Backend. Web chỉ giao tiếp thông qua giao thức HTTP (REST APIs). Code Frontend không chứa logic nghiệp vụ cốt lõi hay thao tác DB trực tiếp tĩnh.

## Các thư mục hỗ trợ khác quan trọng:
- `base_knowledge/`: Chứa các quy chuẩn (standards), rules cho LLM Agents, và tài liệu cấu trúc (structures).
- `openspec/`: Quản lý các tài liệu thiết kế nghiệp vụ, specs, thay đổi (changes) và thiết kế API/Database.
- `.agent/`: Quản lý workflows, kịch bản (skills) để cấu hình cho các AI Agents.

## Lợi ích của kiến trúc tách biệt này:
- **Phát triển song song:** Team Web và Team API có thể làm việc độc lập.
- **Mở rộng dễ dàng:** API có thể được tái sử dụng cho nhiều nền tảng khác (như Mobile App) mà không cần viết lại.
- **Bảo mật:** Database và Business Logic được che giấu kỹ càng đằng sau các lớp API. Front-end không có quyền hạn truy cập trực tiếp vào Back-end.
- **Triển khai (Deployment):** Web và API có thể được host trên các server/container/scaling configuration riêng biệt.
