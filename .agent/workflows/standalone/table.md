---
description: Extract Wiki/Jira requirement to Table create script. Supports dual-mode (pipeline / standalone).
---

# /table-gen - PostgreSQL DDL Generation Pipeline

$ARGUMENTS

---

## Task

This command orchestrates the entire process of generating PostgreSQL DDL scripts. Supports **2 modes**:

### Mode Detection (Tự Động)

| Trigger                                                  | Mode           | Input Source                                                                          |
| -------------------------------------------------------- | -------------- | ------------------------------------------------------------------------------------- |
| `from change <name>`                                     | **Pipeline**   | Đọc `openspec/changes/<name>/design.md` → Section "Entity Design" + "Database Design" |
| `from Jira <ID>` / `from confluence <URL>` / direct args | **Standalone** | Phân tích requirements trực tiếp                                                      |

**Auto-detect rule:** Nếu `$ARGUMENTS` chứa `from change` → Pipeline mode. Ngược lại → Standalone mode.

### Approval Gate Policy

> **🔑 Chỉ có 1 điểm dừng bắt buộc: Step 0 (Execution Plan).**
> Sau khi User approve execution plan, toàn bộ pipeline chạy tự động đến cuối.

---

### Steps:

0. **Execution Planning (Lập Kế Hoạch)**
   - Đọc kỹ `$ARGUMENTS` để nhận dạng mode.
   - **Pipeline mode:** Đọc `openspec/changes/<name>/design.md` → extract Entity Design, DDL specs.
   - **Standalone mode:**
     - Nếu input là yêu cầu thô (Jira/Confluence): TỰ ĐỘNG gọi và thực thi chuỗi workflow `mf` (`mf-init` → `mf-design` → `mf-tasks`) để sinh thiết kế chuẩn trước khi chuyển sang chạy sinh code.
     - Nếu input là thiết kế cấu trúc rõ ràng (File Specs có sẵn): Trực tiếp parse để lập danh sách tables sẽ sinh.
   - **⛔ BẮT BUỘC:** Đọc skill `database-design-postgresql/SKILL.md` (nếu có) trước khi sinh DDL.
   - Trình bày kế hoạch thực thi trực tiếp trên chat (không lưu file `.state/backoffice/execution_plan.md` nữa để tránh rác hệ thống).
   - **🛑 BẮT BUỘC DỪNG (Approval Gate):** Đợi User confirm trước khi sinh DDL.

1. **Table Design (After Plan Approval)**
   - **Pipeline mode:** Skip — design đã có trong `design.md`.
   - **Standalone mode:** Parse thiết kế cung cấp thẳng sang DDL.

2. **DDL Generation**
   - **⛔ BẮT BUỘC đọc TRƯỚC khi sinh DDL:**
     - `database-design-postgresql/SKILL.md` (nếu có) → core rules
     - `base/postgresql-ddl-generation/standard-ddl-example.md` (nếu có) → template
     - `base/postgresql-ddl-generation/sample-data-example.md` (nếu có) → sample data format
   - Sinh DDL scripts theo cấu trúc: Enum/Type (nếu có) → Table → Index → Constraint → Comment
   - **BẮT BUỘC:** Sinh khoảng 11 dòng INSERT dữ liệu mẫu.

3. **Output Delivery**
   - **Pipeline mode:** Lưu `.sql` vào `openspec/changes/<name>/scripts/`
   - **Standalone mode:** Lưu `.sql` vào `.state/backoffice/<topic>/`
   - Ensure `walkthrough.md` checklist đánh dấu `[x]`.

---

## Usage Examples

```
# Pipeline mode — đọc từ change artifacts
/table-gen from change add-customer-feature

# Standalone mode — đọc từ Jira/Confluence/DDL
/table-gen from Jira BO-1023
/table-gen from confluence "https://example.com/abc"
/table-gen
```

---

## Before Starting

If the request is too broad (e.g., just `table-gen` without arguments), ask:

- Please provide the source: Change name, Jira Issue ID, or Confluence URL.
- Is this a brand new domain, or should we inherit from an existing schema?
