---
description: Generate Angular/TypeScript source code (DTO, Module, Service, Component, Routing) following scanned conventions. Supports dual-mode (pipeline / standalone).
---

# /angular-gen — Angular/TypeScript Code Generation Pipeline

$ARGUMENTS

---

## Task

This command orchestrates the process of generating Angular/TypeScript frontend source code (DTO → Module → Service → Component → Routing) that strictly follows established conventions. Supports **2 modes**:

### Mode Detection (Tự Động)

| Trigger | Mode | Input Source |
|---------|------|-------------|
| `from change <name>` | **Pipeline** | Đọc `openspec/changes/<name>/tasks.md` → Frontend tasks + `design.md` → UI specs |
| `from swagger` / `from Jira` / `from confluence` / direct args | **Standalone** | Parse Swagger/SRS trực tiếp |

**Auto-detect rule:** Nếu `$ARGUMENTS` chứa `from change` → Pipeline mode. Ngược lại → Standalone mode.

**⚠️ CRITICAL RULES:**

- Code MUST follow Angular 17 conventions — `inject()`, `signal()`, standalone components.
- Convention files are the **single source of truth** — DO NOT deviate.
- Self-Scan (Step 6) MUST pass with 0 CRITICAL, 0 WARNING before delivery.

### Steps:

0. **Execution Planning (Lập Kế Hoạch Sinh Code)**
   - Đọc kỹ `$ARGUMENTS` để nhận mode.
   - **Pipeline mode:** Đọc `tasks.md` → extract frontend tasks (Layer 7-12). Đọc `design.md` → Frontend Design section.
   - **Standalone mode:** 
     - Nếu input là yêu cầu thô (Jira/Confluence/Tên Feature): TỰ ĐỘNG gọi và thực thi chuỗi workflow `mf` (`mf-init` → `mf-design` → `mf-tasks`) để thiết kế chuẩn trước khi sinh code.
     - Nếu input là thiết kế API rõ ràng (Swagger JSON): Trực tiếp parse để lập kế hoạch sinh code.
   - Xác định feature name, endpoint list, scope.
   - **⛔ BẮT BUỘC:** Load `angular-code-generation/SKILL.md` trước khi sinh code.
   - Trình bày kế hoạch thực thi trực tiếp trên chat (không lưu file `.state/backoffice/gen-fe/execution_plan.md`).
   - **TẠM DỪNG (Approval Gate):** Đợi User confirm trước khi sinh.

1. **DTO Generation (After Plan Approval)**
   - **Skill:** `angular-code-generation/dto-gen.md`
   - **Task:** Sinh Request/Response TypeScript interfaces matching backend DTOs.
   - **Output:** `src/app/business-modules/{feature}/models/{feature}.dto.ts`

2. **Module & Routing Generation**
   - **Skill:** `angular-code-generation/module-gen.md`
   - **Task:** Sinh feature module + routing. Setup lazy loading.
   - **Output:** `src/app/business-modules/{feature}/{feature}.module.ts`

3. **Service Generation**
   - **Skill:** `angular-code-generation/service-gen.md`
   - **Task:** Sinh API service với `inject(HttpClient)`, CRUD methods.
   - **Output:** `src/app/business-modules/{feature}/services/{feature}.service.ts`

4. **Component Generation**
   - **Skill:** `angular-code-generation/list-component-gen.md` + `form-component-gen.md`
   - **Task:** Sinh List component (`signal()`, `TableConfig`) + Form component (Reactive Forms).
   - **Output:** `src/app/business-modules/{feature}/components/list/`, `components/form/`

5. **Shared Integration**
   - **Skill:** `angular-code-generation/shared-integration-gen.md`
   - **Task:** Tạo enum files, constants, cập nhật parent routing (lazy loading).
   - **Output:** shared enums, constants, parent routing update

6. **Self-Scan & Verification**
   - **Skill:** `angular-code-generation/self-scan.md`
   - **Task:** Chạy convention checker. Expected: 0 CRITICAL, 0 WARNING.
   - **Output:** `.state/backoffice/gen-fe/scan_result.md`
   - **Pipeline mode bonus:** Mark completed tasks as `[x]` trong `tasks.md`.

---

## Usage Examples

```
# Pipeline mode — đọc từ change artifacts
/angular-gen from change add-customer-feature

# Standalone mode
/angular-gen from swagger ./swagger.json endpoint /api/city
/angular-gen from Jira BO-1024
/angular-gen from confluence https://example.com/page
/angular-gen module city (from existing service)
/angular-gen dto-only from swagger ./swagger.json
/angular-gen component-only city
```

---

## Before Starting

If the request is too broad (e.g., just `angular-gen` without arguments), ask:
- Source: Change name / Swagger JSON / Jira Issue / Confluence URL?
- Feature name?
- Scope: full pipeline (DTO → Component) hay bắt đầu từ step nào?
