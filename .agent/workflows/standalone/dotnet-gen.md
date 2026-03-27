---
description: Generate .NET code (Entity, DTO, Interface, Service, Controller) following scanned conventions using Serena. Supports dual-mode (pipeline / standalone).
---

# /dotnet-gen — .NET Code Generation Pipeline (Serena Integrated)

$ARGUMENTS

---

## Serena Tool Integration 🧠
This generator utilizes **Serena MCP tools** for semantic code navigation, convention enforcement, and precise code manipulation instead of raw file writes.

| Tool | Category | Purpose |
|------|----------|---------|
| `check_onboarding_performed` | workflow | Verify project was onboarded |
| `list_memories` & `read_memory` | memory | Load code generation conventions |
| `find_symbol` & `get_symbols_overview` | symbol | Understand existing structure and pick references |
| `create_text_file` | file | Create new C# files |
| `replace_symbol_body` & `insert_after_symbol` | symbol | Edit existing methods/classes without full overwrite |
| `execute_shell_command` | cmd | Run `dotnet build` post-generation |

---

## Task

This command orchestrates the process of generating .NET source code (Entity → DTO → Interface → Service → Controller) that strictly follows established conventions. Supports **2 modes**:

### Mode Detection (Tự Động)

| Trigger | Mode | Input Source |
|---------|------|-------------|
| `from change <name>` | **Pipeline** | Đọc `openspec/changes/<name>/tasks.md` → Backend tasks + `design.md` → class specs |
| `from DDL` / `from Jira` / `from confluence` / direct args | **Standalone** | Parse DDL/SRS trực tiếp |

**Auto-detect rule:** Nếu `$ARGUMENTS` chứa `from change` → Pipeline mode. Ngược lại → Standalone mode.

**⚠️ CRITICAL RULES:**

- **Onboarding Check First:** MUST call `check_onboarding_performed()` before planning.
- Code MUST use **direct DI** (constructor injection) — **NEVER** use `IServiceProvider`.
- Convention files (loaded via `read_memory` or `read_file`) are the **single source of truth** — DO NOT deviate.
- Post-generation verification MUST use semantic tools (`get_symbols_overview`) to ensure compliance.

### Steps:

0. **Onboarding & Execution Planning**
   - **Invoke:** `check_onboarding_performed()`. If missing, run `onboarding()`.
   - Đọc kỹ `$ARGUMENTS` để nhận mode.
   - **Pipeline mode:** Đọc `tasks.md` → extract backend tasks. Đọc `design.md` → class specs.
   - **Standalone mode:** Parse input → TỰ ĐỘNG gọi `mf-init` → `mf-design` → `mf-tasks` nếu cần thiết kế trước.
   - **Load Memories:** Dùng `list_memories()` → `read_memory()` để load các convention liên quan đến Entity, DTO, Service, Controller.
   - Trình bày kế hoạch thực thi trên chat. Đợi User approval (Approval Gate) trước khi chạy các step sau.

1. **Entity Generation**
   - **Skill:** Load convention `dotnet-entity-convention` và `dotnet-code-generation/entity-gen.md`.
   - **Action:** Dùng `create_text_file("BOBase.Domain/Entities/{Module}/{Name}Entity.cs", "...")`
   - **Verify:** Dùng `get_symbols_overview` hoặc báo cáo để check attributes `[Table]`/`[Column]`.

2. **DTO Generation**
   - **Skill:** Load convention `dotnet-dto-convention` và `dotnet-code-generation/dto-gen.md`.
   - **Action:** Dùng `create_text_file` sinh Request (kế thừa `FilterRequest`) & Response.
   - **Verify:** `get_symbols_overview` để đảm bảo property name convention khớp.

3. **Interface Generation**
   - **Skill:** Load `dotnet-interface-convention`.
   - **Action:** Dùng `create_text_file("modules/BOModule.{Name}/Services/I{Name}Service.cs", "...")`

4. **Service Generation**
   - **Skill:** Load `dotnet-service-convention` và `dotnet-code-generation/service-gen.md`.
   - **Context Lookup:** Dùng `find_symbol("IScoped")` hoặc tìm base class tương tự nếu cần.
   - **Action:** `create_text_file("modules/BOModule.{Name}/Services/Impls/{Name}Service.cs", "...")`
   - **Verify:** Dùng `get_symbols_overview` để đảm bảo constructor injection DI chuẩn, KHÔNG có `IServiceProvider`.

5. **Controller Generation**
   - **Skill:** Load `dotnet-controller-convention`.
   - **Context Lookup:** Dùng `find_symbol("BaseController")` tìm controller mẫu để tham khảo routing/auth style.
   - **Action:** Dùng `create_text_file("modules/BOModule.{Name}/Controllers/{Name}Controller.cs", "...")`
   - **Update Constants:** Thay vì ghi đè toàn bộ file, ưu tiên `insert_after_symbol` hoặc thay văn bản an toàn cho `Constants/FunctionConst.cs`.

6. **Self-Scan & Verification**
   - **Action:** Dùng `execute_shell_command("dotnet build")` để verify lỗi biên dịch compile.
   - **Fix Errors:** Mọi syntax error / DI error phải được sửa chữa bằng semantic tools `replace_symbol_body` hoặc `insert_after_symbol` thay vì ghi lại cả file.
   - Báo cáo kết quả trực tiếp cho user (0 CRITICAL, 0 WARNING).

---

## Usage Examples

```bash
# Pipeline mode — auto code generation from tasks.md & design.md
/dotnet-gen from change add-customer-feature

# Standalone mode
/dotnet-gen from DDL (paste CREATE TABLE script)
/dotnet-gen from Jira BO-1024
/dotnet-gen from confluence https://example.com/page
/dotnet-gen module Background (from existing entity)
/dotnet-gen entity-only from DDL
/dotnet-gen dto-and-below Background
```

---

## Before Starting

If the request is too broad (e.g., just `dotnet-gen` without arguments), ask:
- Source: Change name / DDL script / Jira Issue / Confluence URL?
- Module name?
- Scope: full pipeline (Entity → Controller) hay bắt đầu từ step nào?
