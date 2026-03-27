# Rule 14: .NET Source Code Scan Rules

> **Quy tắc bắt buộc khi thực hiện scan source .NET. Agent scanner PHẢI tuân thủ nghiêm ngặt.**

---

## 0. Dynamic Path Resolution

`${PROJECT_ROOT}` là thư mục chứa file `.sln` của project. Agent BẮT BUỘC:

1. Tìm file `*.sln` trong workspace hiện tại.
2. Thư mục chứa file `.sln` đó chính là `${PROJECT_ROOT}`.
3. Thay thế tất cả `${PROJECT_ROOT}` bằng path thực tế trước khi scan.

---

## 1. Phạm Vi Scan (Scan Scope)

### Directories IN SCOPE:

| Directory                           | Content                       | Priority  |
| ----------------------------------- | ----------------------------- | --------- |
| `${PROJECT_ROOT}/src/Base.Domain/`  | Entities, Enums, Base classes | 🔴 HIGH   |
| `${PROJECT_ROOT}/modules/Module.*/` | DTOs, Services, Mappings      | 🔴 HIGH   |
| `${PROJECT_ROOT}/api/`              | Controllers                   | 🟠 MEDIUM |

### Files IN SCOPE:

- Chỉ scan file có extension `.cs`.
- Bỏ qua thư mục `bin/`, `obj/`, `.vs/`.

### Files OUT OF SCOPE:

- `*.csproj`, `*.json`, `*.config`, `*.sln`
- Test files (nếu có)
- Generated code (auto-generated files)

---

## 2. Convention Baseline

**Source of Truth DUY NHẤT:** Các sub-skill files trong `skills/dotnet-convention-checker/`. Mỗi sub-skill đã self-contained — chứa đầy đủ rules, checklist, và examples.

Agent PHẢI đọc và nội hóa toàn bộ các section sau TRƯỚC khi scan:

| Section                   | Nội dung kiểm tra                                                            |
| ------------------------- | ---------------------------------------------------------------------------- |
| §1. Naming Conventions    | Entity naming, Property naming, Column mapping, DTO naming, Interface naming |
| §2. Entity Design Pattern | Base hierarchy, Relationship patterns, Data annotations                      |
| §3. DTO Strategy          | Request types, Folder structure, DTO conventions                             |
| §4. Code Templates        | Standard entity template, Approvable entity template, DTO templates          |

---

## 3. Severity Classification

| Level    | Icon | Mô tả                                                       | Ví dụ                                                      |
| -------- | ---- | ----------------------------------------------------------- | ---------------------------------------------------------- |
| CRITICAL | 🔴   | Vi phạm nghiêm trọng, ảnh hưởng runtime hoặc data integrity | Missing [Key], wrong base class, no [Table] attribute      |
| WARNING  | 🟠   | Vi phạm convention rõ ràng, không gây crash                 | DTO naming sai pattern, missing [JsonIgnore] on navigation |
| INFO     | 🟡   | Vi phạm nhỏ hoặc recommendation                             | Property naming style, missing optional validation         |
| PASS     | ✅   | Tuân thủ đầy đủ convention                                  | Đúng tất cả convention checks                              |

---

## 4. Output Format Chuẩn

### 4.1 Per-Module Summary Table

```markdown
## Module: Module.IconManagement

| Category | 🔴 CRITICAL | 🟠 WARNING | 🟡 INFO | ✅ PASS | Total Files |
| -------- | ----------- | ---------- | ------- | ------- | ----------- |
| Entity   | 0           | 2          | 3       | 5       | 10          |
| DTO      | 1           | 4          | 2       | 8       | 15          |
| Service  | 0           | 1          | 0       | 3       | 4           |
```

### 4.3 Overall Summary with Chart

```markdown
## Overall Compliance Report

Total files scanned: {N}

| Severity    | Count | %   |
| ----------- | ----- | --- |
| 🔴 CRITICAL | X     | X%  |
| 🟠 WARNING  | Y     | Y%  |
| 🟡 INFO     | Z     | Z%  |
| ✅ PASS     | W     | W%  |
```

Kèm Mermaid Pie Chart:

```mermaid
pie title Scan Compliance
    "PASS" : W
    "INFO" : Z
    "WARNING" : Y
    "CRITICAL" : X
```

---

## 5. Quy Tắc Hành Vi (Behavioral Rules)

| #   | Rule                   | Mô tả                                                                                                          |
| --- | ---------------------- | -------------------------------------------------------------------------------------------------------------- |
| R1  | **READ-ONLY**          | NGHIÊM CẤM sửa bất kỳ file source nào. Chỉ đọc và báo cáo.                                                     |
| R2  | **EXHAUSTIVE**         | Phải scan HẾT file trong scope, không được bỏ sót.                                                             |
| R3  | **TRACEABLE**          | Mỗi finding PHẢI reference đến checklist ID cụ thể trong sub-skill file tương ứng (e.g. E1–E10, R1–R8, D1–D8). |
| R4  | **STRUCTURED**         | Output PHẢI theo format chuẩn ở Section 4.                                                                     |
| R5  | **NO FALSE POSITIVES** | Verify kỹ trước khi report. So sánh Expected vs Actual.                                                        |
| R6  | **PRIORITIZED**        | Report findings theo thứ tự: CRITICAL → WARNING → INFO.                                                        |

---

## 6. Anti-Patterns

❌ Scan xong nhưng không tạo report file.
❌ Report violation mà không reference rule section.
❌ Tự ý sửa code khi phát hiện violation.
❌ Bỏ qua module vì "quá nhiều file".
❌ Không phân loại severity cho finding.
