# Vibe Coding: Check Scam Project

Welcome to the **Check Scam** project! This repository is built using the **Vibe Coding** methodology, leveraging **OpenSpec** for dynamic requirement specifications and the **Antigravity Kit** (AI Agent Assistant) to drive automated development, code scanning, và duy trì tính nhất quán của hệ thống.

## 🚀 Tổng quan (Overview)

Dự án này áp dụng luồng phát triển phần mềm AI-assisted (AI hỗ trợ chủ động). Bằng cách sử dụng các AI Agent dựa trên LLM (Antigravity Kit), chúng tôi tự động hóa các tác vụ từ phân tích yêu cầu, sinh database, xây dựng API, cho đến code review và quét bảo mật.

- **Phương pháp luận (Methodology)**: Vibe Coding (Lập trình dựa trên định hướng LLM)
- **Quản lý thiết kế (Spec Management)**: OpenSpec
- **Công cụ AI (AI Tooling)**: Antigravity Kit

## 📂 Cấu trúc dự án (Project Structure)

Dự án được chia module rõ ràng nhằm cô lập hoàn toàn giữa Frontend, Backend, tài liệu thiết kế nghiệp vụ và cấu hình Agent:

- `api/`: Mã nguồn Backend (được xây dựng bằng C# ASP.NET Core). Các service API cốt lõi nằm ở `api/api-vibe`.
- `web/`: Mã nguồn Web Frontend (Angular).
- `openspec/`: Chứa các đặc tả API (`yaml`), schemas, tài liệu thiết kế và yêu cầu thay đổi (change proposals). AI Agent sẽ đọc OpenAPI và specs ở đây để hiểu task trước khi sinh code.
- `base_knowledge/`: Nền tảng tri thức trung tâm. Chứa các quy chuẩn code (coding standards), hướng dẫn kiến trúc, quy tắc quét bảo mật (security/convention checker) mà AI Agent PHẢI tuân theo nghiêm ngặt.
- `.agent/`: Định nghĩa các Workflows và Skills cho Antigravity Kit. Hỗ trợ agent thực thi các tác vụ như `/table-gen` (sinh DDL PostgreSQL/Oracle), scan source .NET, v.v.

## 🤖 Tích hợp Antigravity & OpenSpec

Trong dự án này, quy trình lập trình được thay đổi:
1. **Thiết kế** (Define): Viết và đặc tả yêu cầu, API schemas bên trong `openspec/`.
2. **Quy chuẩn** (Guardrails): Áp dụng các rules chặt chẽ từ thư mục `base_knowledge/` (ví dụ: `PRJ-03-dotnet-scan-rule`, `PRJ-04-angular-scan-rule`).
3. **Thực thi** (Command): Ra lệnh cho Antigravity Agent thông qua các slash commands. Agent sẽ tự động đọc spec, quét rule, phân tích và sinh ra code đáp ứng production vào `api/` hoặc `web/`.

## 🛠️ Hướng dẫn khởi chạy (Getting Started)

### Yêu cầu hệ thống (Prerequisites)
- SDK .NET tương ứng (dành cho API Backend)
- Node.js & Angular CLI (dành cho Web Frontend)
- Môi trường đã bật/cài đặt **Gemini Antigravity Kit**

### Chạy Backend (API)
```bash
cd api/api-vibe
dotnet restore
dotnet run
```

### Chạy Frontend (Web)
*(Thư mục Web hiện đang được thiết lập dự kiến)*
```bash
cd web
npm install
npm start
```

## 🛡️ Bảo mật & Tiêu chuẩn (Security & Standards)
Toàn bộ code được sinh ra hoặc commit đều có thể được tự động quét (scan) đối chiếu với các `common_rules` nằm tại `base_knowledge/`. Dự án chủ trương Clean Code, không lưu trữ thông tin nhạy cảm (hardcoded secrets, tokens, connection strings) ở dạng plain text, và tách biệt rạch ròi trách nhiệm giữa các tầng ứng dụng.

---
*Dự án được phát triển và tối ưu thông qua trải nghiệm lập trình AI-assisted.*
