---
name: api-patterns
description: REST API design principles and conventions. Resource naming, HTTP method mapping, response format, authentication patterns, API documentation, security testing.
allowed-tools: Read, Write, Edit, Glob, Grep
version: 2.0
priority: HIGH
---

# API Patterns — REST API Design Principles

> REST API convention chung cho mọi dự án.
> **Đọc đúng file cần thiết theo bảng ánh xạ bên dưới.**

## 🎯 Selective Reading Rule

**Read ONLY files relevant to the request!** Check the content map, find what you need.

> 💡 **Tech-specific convention** (code examples, framework patterns) nằm tại **project level**: xem `backoffice/skills/api-patterns/`

---

## Core REST Principles

### 1. Resource Naming

| Rule | ✅ Đúng | ❌ Sai |
|---|---|---|
| Dùng **danh từ** (không động từ) | `POST /api/users` | `POST /api/createUser` |
| Lowercase, không dấu | `/api/backgrounds` | `/api/Backgrounds` |
| Nested cho quan hệ cha-con | `/api/categories/{id}/items` | `/api/getCategoryItems` |
| Tối đa 2 cấp lồng | `/api/modules/{id}/functions` | `/api/a/{id}/b/{id}/c/{id}/d` |

### 2. HTTP Method → Action Mapping

| HTTP Method | Action | Mục đích |
|---|---|---|
| `GET` | Search / GetBy | Đọc dữ liệu (list hoặc single) |
| `POST` | Create | Tạo mới resource |
| `PUT` | Update | Cập nhật toàn bộ resource |
| `PATCH` | Partial Update / Toggle | Cập nhật một phần (status, etc.) |
| `DELETE` | Delete | Xóa resource |

### 3. Status Code Usage

| Tình huống | Status Code |
|---|---|
| Thành công (có data) | `200 OK` |
| Tạo mới thành công | `201 Created` |
| Input validation lỗi | `400 Bad Request` |
| Chưa đăng nhập | `401 Unauthorized` |
| Không có quyền | `403 Forbidden` |
| Không tìm thấy | `404 Not Found` |
| Lỗi server | `500 Internal Server Error` |

### 4. Response Format Principles

- Mọi response PHẢI dùng **format nhất quán** (envelope pattern hoặc standard format riêng project)
- List API PHẢI có **pagination** chuẩn (totalRecords, pageNumber, pageSize)
- Error response PHẢI có **structure rõ ràng** (status code, message, validation errors)
- Frontend và Backend PHẢI thống nhất response contract

> 📋 Response format cụ thể (JSON structure, envelope fields) xem tại **project level**: `response.md`

### 5. Authentication & Authorization Principles

- Mọi API endpoint PHẢI có authorization check
- Dùng token-based auth (JWT) với role-based access control (RBAC)
- Permission phải đồng bộ 3 nơi: Database ↔ Backend ↔ Frontend
- Không lưu sensitive data trong token

### 6. API Documentation Principles

- API documentation là "hợp đồng" giữa Backend và Frontend
- Mọi endpoint PHẢI có: summary, parameters, response schema, status codes
- DTO properties PHẢI có mô tả + example values
- Documentation viết theo ngôn ngữ business, không mô tả implementation

---

## ✅ Decision Checklist

Before designing an API:

- [ ] **Resource URL** đúng convention? (danh từ, lowercase, follow existing pattern)
- [ ] **Response format** dùng envelope pattern nhất quán?
- [ ] **Pagination** dùng standard pagination format cho list APIs?
- [ ] **Authorization** trên mọi endpoint?
- [ ] **Permission sync** — DB, BE, FE cùng key?
- [ ] **API Documentation** — summary + response schema đầy đủ?
- [ ] **DTO documentation** — mỗi property có mô tả + example?

---

## ❌ Anti-Patterns

**DON'T:**
- Dùng động từ trong URL (`/api/getUser` → `/api/users`)
- Return raw framework responses — dùng envelope pattern
- Hardcode permission strings — dùng constants
- Thiếu API documentation
- Expose internal errors/stack traces cho client
- Business logic trong Controller — delegate sang Service

**DO:**
- Follow existing naming pattern trong project
- Dùng pagination format chuẩn cho mọi list API
- Sync permission 3 nơi: DB, BE, FE
- API documentation bằng ngôn ngữ team hiểu
- Document enum/status values rõ ràng

---

## 🔧 Project Override Guide

> **Khi setup dự án mới**, scan project và tạo file override sau tại `<project>/skills/api-patterns/`:

| File cần tạo | Nội dung cần scan | Cách tạo |
|-------------|-------------------|----------|
| `rest.md` | Controller patterns, URL conventions, HTTP method mapping | Scan existing controllers → document REST convention cụ thể (naming, base controller, action methods) |
| `response.md` | Response envelope format, pagination structure, error format | Scan base response classes → document JSON response format |
| `auth.md` | Auth mechanism (JWT/OAuth2/Session), RBAC patterns, permission sync | Scan auth middleware + permission constants → document auth flow |
| `documentation.md` | API doc tool (Swagger/OpenAPI), annotation patterns | Scan existing API docs → document annotation standards |
| `security-testing.md` | OWASP top 10 context, banking-specific checks | Scan tech stack + domain → tạo security testing checklist |

**Scan command gợi ý:**
```
Scan controllers/ + middleware/ + response classes → tạo rest.md, response.md, auth.md, documentation.md, security-testing.md
```
