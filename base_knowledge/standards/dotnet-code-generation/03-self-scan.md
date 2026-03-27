---
name: self-scan
description: Chạy convention checker trên code vừa sinh để verify compliance. Tham chiếu dotnet-convention-checker/SKILL.md.
version: 1.0
---

# Self-Scan Sub-Skill

> **BẮT BUỘC đọc trước:** `../dotnet-convention-checker/SKILL.md` — full 7-step checklist

---

## Purpose

Sau khi sinh code xong (Step 1–5), chạy Self-Scan để verify code vừa tạo tuân thủ đúng convention.

---

## Execution

### 1. Scan Scope

Chỉ scan files VỪA SINH trong pipeline, không scan toàn bộ codebase:

| Generated File                                           | Convention Checklist                   |
| -------------------------------------------------------- | -------------------------------------- |
| `{Name}Entity.cs`                                        | E1–E10 (entity) + R1–R8 (relationship) |
| `{Name}FilterRequest.cs`, `{Name}CreateRequest.cs`, etc. | D1–D10 (dto)                           |
| `I{Name}Service.cs`                                      | I1–I7 (interface)                      |
| `{Name}Service.cs`                                       | S1–S9 (service)                        |
| `{Name}Mapper.cs`                                        | M1–M10 (mapping)                       |
| `{Name}Controller.cs`                                    | CT1–CT10 (controller)                  |

### 2. Expected Result

| Severity    | Expected Count     |
| ----------- | ------------------ |
| 🔴 CRITICAL | **0**              |
| 🟠 WARNING  | **0**              |
| 🟡 INFO     | **0** (nếu có thể) |
| ✅ PASS     | **100%**           |

### 3. If Findings Found

- **CRITICAL / WARNING:** PHẢI fix ngay trước khi trình User.
- Quay lại step tương ứng, sửa code, rồi chạy lại Self-Scan.
- Lặp cho đến khi 0 CRITICAL + 0 WARNING.

---

## Output

- File: `scan_result.md` (hiển thị trực tiếp hoặc lưu theo file nếu có workflow)
- Format: Giống `scan_report.md` của `/scan-source`
- Bao gồm: summary table, per-file checklist, pass/fail status

---

## Template Output

```markdown
# Self-Scan Report — {Module} Code Generation

> **Date:** {date} | **Files scanned:** {N}

## Summary

| File                   | Checklist | 🔴  | 🟠  | 🟡  | ✅  |
| ---------------------- | --------- | --- | --- | --- | --- |
| {Name}Entity.cs        | E1–E10    | 0   | 0   | 0   | 10  |
| {Name}FilterRequest.cs | D1–D10    | 0   | 0   | 0   | 10  |
| I{Name}Service.cs      | I1–I7     | 0   | 0   | 0   | 7   |
| {Name}Service.cs       | S1–S9     | 0   | 0   | 0   | 9   |
| {Name}Controller.cs    | CT1–CT10  | 0   | 0   | 0   | 10  |

**Result: ✅ ALL PASS**
```
