# Rule 16: Angular/TypeScript Source Code Scan Rules

> **Quy tắc bắt buộc khi thực hiện scan source Angular/TypeScript. Agent scanner PHẢI tuân thủ nghiêm ngặt.**

---

## 1. Phạm Vi Scan (Scan Scope)

### Directories IN SCOPE:

| Directory                          | Content                                          | Priority  |
| ---------------------------------- | ------------------------------------------------ | --------- |
| `src/app/pages/`                   | Feature modules, Components, Routing             | 🔴 HIGH   |
| `src/app/shared/services/api/`     | API Services (extends GeneralService)            | 🔴 HIGH   |
| `src/app/shared/components/`       | Shared custom components                         | 🔴 HIGH   |
| `src/app/shared/`                  | Enums, constants, pipes, directives, models      | 🟠 MEDIUM |
| `src/app/cores/guards/`            | Functional guards (auth, gatekeeper, privileges) | 🔴 HIGH   |
| `src/app/cores/interceptors/`      | HTTP interceptors                                | 🔴 HIGH   |
| `src/app/shared/services/utils/`   | Utility services (non-API)                       | 🟠 MEDIUM |
| `src/app/shared/services/helpers/` | Helper services                                  | 🟠 MEDIUM |
| `src/app/shared/helpers/`          | Pure helper functions                            | 🟠 MEDIUM |
| `src/app/layout/`                  | Layout components                                | 🟡 LOW    |

### Files IN SCOPE:

- Scan file có extension `.ts`, `.html`, `.scss`.
- Bỏ qua thư mục `node_modules/`, `.angular/`, `dist/`, `.git/`.

### Files OUT OF SCOPE:

- `*.json`, `*.config.js`, `*.spec.ts` (test files)
- `environments/**` (environment configs)
- `assets/**` (static assets)
- Generated code (auto-generated files)

---

## 2. Convention Baseline

**Source of Truth DUY NHẤT:** Các sub-skill convention files trong `base_knowledge/standards/angular-convention-checker/`

Agent PHẢI đọc và nội hóa toàn bộ sub-skills theo mapping table trong `SKILL.md` TRƯỚC khi scan:

| Sub-Skill                                 | Nội dung kiểm tra                                                               |
| ----------------------------------------- | ------------------------------------------------------------------------------- |
| `angular-component-convention.md`         | @Component decorator, selector, lifecycle hooks, DI injection, standalone       |
| `angular-module-convention.md`            | @NgModule, declarations, imports, exports, lazy-loading, folder structure       |
| `angular-service-convention.md`           | @Injectable, extends GeneralService, apiName, no CRUD override                  |
| `angular-routing-convention.md`           | Lazy loading, route guard, path naming, child routes                            |
| `angular-template-convention.md`          | Structural directives, ngFor trackBy, async pipe, no business logic in template |
| `angular-shared-convention.md`            | Components/directives/pipes/enums/constants organization, reusable pattern      |
| `angular-naming-convention.md`            | File naming, class naming, method naming, folder structure                      |
| `angular-guard-interceptor-convention.md` | Functional guards, interceptors, token refresh, OAuth2 SSO                      |
| `angular-utility-service-convention.md`   | Signal-based services, dynamic components, validators, event bus                |
| `angular-helper-convention.md`            | Pure function helpers, naming, placement rules                                  |
| `angular-shared-component-api.md`         | Shared component API contracts: inputs, outputs, usage guidelines               |

---

## 3. Severity Classification

| Level    | Icon | Mô tả                                                       | Ví dụ                                                           |
| -------- | ---- | ----------------------------------------------------------- | --------------------------------------------------------------- |
| CRITICAL | 🔴   | Vi phạm nghiêm trọng, ảnh hưởng runtime hoặc data integrity | Missing decorator, memory leak (no unsubscribe), broken routing |
| WARNING  | 🟠   | Vi phạm convention rõ ràng, không gây crash                 | Naming sai pattern, missing trackBy, wrong folder placement     |
| INFO     | 🟡   | Vi phạm nhỏ hoặc recommendation                             | Minor naming style, missing optional lifecycle hook             |
| PASS     | ✅   | Tuân thủ đầy đủ convention                                  | Đúng tất cả convention checks                                   |

---

## 4. Output Format Chuẩn

### 4.1 Per-Module Summary Table

```markdown
## Module: pages/category

| Category  | 🔴 CRITICAL | 🟠 WARNING | 🟡 INFO | ✅ PASS | Total Files |
| --------- | ----------- | ---------- | ------- | ------- | ----------- |
| Component | 0           | 2          | 3       | 5       | 10          |
| Module    | 0           | 1          | 0       | 2       | 3           |
| Routing   | 1           | 0          | 1       | 1       | 3           |
| Template  | 0           | 3          | 2       | 5       | 10          |
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

| #   | Rule                   | Mô tả                                                                                    |
| --- | ---------------------- | ---------------------------------------------------------------------------------------- |
| R1  | **READ-ONLY**          | NGHIÊM CẤM sửa bất kỳ file source nào. Chỉ đọc và báo cáo.                               |
| R2  | **EXHAUSTIVE**         | Phải scan HẾT file trong scope, không được bỏ sót.                                       |
| R3  | **TRACEABLE**          | Mỗi finding PHẢI reference đến section cụ thể trong sub-skill convention file tương ứng. |
| R4  | **STRUCTURED**         | Output PHẢI theo format chuẩn ở Section 4.                                               |
| R5  | **NO FALSE POSITIVES** | Verify kỹ trước khi report. So sánh Expected vs Actual.                                  |
| R6  | **PRIORITIZED**        | Report findings theo thứ tự: CRITICAL → WARNING → INFO.                                  |

---

## 6. Anti-Patterns

❌ Scan xong nhưng không tạo report file.
❌ Report violation mà không reference rule section.
❌ Tự ý sửa code khi phát hiện violation.
❌ Bỏ qua module vì "quá nhiều file".
❌ Không phân loại severity cho finding.
