---
name: security-compliance-checker
description: Security compliance scanning skill. Dynamically extracts rules from user-provided Excel file (auto-detects format), provides scan methodology for Backend and Frontend code.
---

# Security Compliance Checker

> **Skill rà soát tuân thủ bảo mật** dựa trên file Excel do user cung cấp.
> Script **tự động phát hiện** cấu trúc cột — không giả định tên file hay format cố định.

## Tổng Quan

Skill này hỗ trợ agent quét source code (Backend + Frontend) để kiểm tra tuân thủ các yêu cầu bảo mật. Quy trình gồm 8 phases:

1. **Phase 0 — Get File Path**: Hỏi user đường dẫn file Excel
2. **Phase 1 — Rule Extraction**: Chạy script đọc Excel → checklist
3. **Phase 2 — User Approval**: User review và approve checklist
4. **Phase 3 — Init**: Load approved checklist + methodology, resolve paths
5. **Phase 4 — Backend Scan**: Scan controllers, services, config, middleware
6. **Phase 5 — Frontend Scan**: Scan components, services, guards, templates
7. **Phase 6 — Cross-cutting**: Session, token, file handling, error, libs
8. **Phase 7 — Report**: Classify findings, generate report

---

## Phase 0: Get File Path

**BẮT BUỘC hỏi user trước.** Không giả định tên file.

---

## Phase 1: Rule Extraction

```bash
python base_knowledge/standards/security-compliance-checker/scripts/extract-owasp-rules.py "<user_provided_path>" --output ./owasp_checklist.md
```

Script tự động:
- Tìm header row (scan 15 rows đầu)
- Auto-detect cột bằng fuzzy matching (hỗ trợ cả tiếng Việt và tiếng Anh)
- Filter theo PIC (default: All + BO, tùy chỉnh via `--pic`)
- Xuất markdown checklist

**Debug tools:**
- `--list-sheets` — liệt kê tất cả sheets
- `--preview` — xem cấu trúc sheet + kết quả auto-detect

> ⚠️ Cần `openpyxl`: `pip install openpyxl`

---

## Phase 2: User Approval (MANDATORY GATE)

**KHÔNG ĐƯỢC scan khi chưa có approval.**
1. Báo user: tổng rules, chapters, PIC breakdown.
2. Hỏi: "Checklist có đúng không?"
3. Chỉ tiếp Phase 3 khi user xác nhận.

---

## Sub-Skill Files (Scan Methodology)

Chứa **HOW to check** (grep patterns, code examples GOOD/BAD).
**WHAT to check** đến từ `owasp_checklist.md` (approved by user).

| # | File | Chapters | Focus |
|---|------|----------|-------|
| 1 | `methodology-injection-sanitization.md` | V1 | Injection, deserialization |
| 2 | `methodology-web-api-security.md` | V3, V4 | CORS, CSP, HSTS, WebSocket |
| 3 | `methodology-auth-session-token.md` | V6–V9 | Auth, session, JWT, RBAC |
| 4 | `methodology-file-crypto-infra.md` | V5, V11–V14 | File, TLS, data protection |
| 5 | `methodology-coding-error.md` | V15, V16 | Libs, error handling |

---

## Dependencies

- Python 3.x + `openpyxl`
- File Excel do user cung cấp (bất kỳ tên + format nào)
