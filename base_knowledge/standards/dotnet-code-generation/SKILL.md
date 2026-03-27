---
name: dotnet-code-generation
description: Sinh code .NET (Entity, DTO, Interface, Service, Controller) tuân thủ convention đã chuẩn hóa. Hỗ trợ 2 mode — CRUD (từ DDL/SRS) và REPORT (từ Oracle Package .pks/.pkb). Mỗi step tham chiếu convention file tương ứng từ dotnet-convention-checker/.
allowed-tools: Read, Write, Grep, Glob
version: 2.0
priority: CRITICAL
---

# .NET Code Generation Skill

> **Sinh code .NET đúng chuẩn. Hỗ trợ 2 mode: CRUD và REPORT. Mỗi gen step BẮT BUỘC đọc convention tương ứng TRƯỚC khi sinh code. Single source of truth: `dotnet-convention-checker/`.**

## 🔀 CRUD vs REPORT Decision Table (ĐỌC ĐẦU TIÊN)

| Tiêu chí | CRUD Mode | REPORT Mode |
|-----------|-----------|-------------|
| **Input** | DDL script (`CREATE TABLE`) + SRS | Oracle Package file (`.pks`/`.pkb`) |
| **Module** | `BOModule.{Feature}` (Category, Customer...) | `BOModule.Report` |
| **Entity** | ✅ Sinh Entity class | ❌ KHÔNG có Entity |
| **DbSet** | ✅ Đăng ký DbContext | ❌ KHÔNG có DbContext |
| **DTO Request** | Extends `FilterRequest` → inject từ query params | Extends `{Name}Setting` → `GetParameters()` trả `OracleParameter[]` |
| **DTO Response** | Extends `BaseResponse` / custom | Extends `BaseProcModel` + `[JsonProperty("COL")]` + `[ClientTableConfig]` |
| **Interface** | `I{Name}Service` extends custom | `I{Name}Service : IBaseReportService<TFilter, TResponse>` |
| **Service** | Custom logic, `I{Name}Service, IScoped` | `BaseCrossReportService<TFilter, TResponse>, IScoped, I{Name}Service` (no custom logic) |
| **Controller** | Multiple CRUD actions (Search/GetBy/Create/Update/Delete) | 2 actions only: `Search` + `Export` |
| **FunctionConst** | `Func{Feature}{Action}Id` (nhiều entries) | `Func{Name}Id` + `Func{Name}ExportId` (2 entries) |
| **Extra files** | Mapping class | Setting class + EPackage enum value |

> **Quy tắc chung DÙNG CHUNG cả 2 mode:**
> - DI trực tiếp (constructor injection) — TUYỆT ĐỐI KHÔNG dùng `IServiceProvider`
> - `IScoped` marker interface cho Service
> - `BaseController` cho Controller
> - `[Authorize(Roles = FunctionConst.xxx)]` trên mỗi action
> - `return Response(...)` pattern
> - Thin controller principle

---

## 🔍 Detail Variant Decision Map (Nhận biết khi SRS có "Xem chi tiết")

> **Khi nào cần đọc?** Khi SRS có mô tả "Xem chi tiết" / "Detail" / "Thao tác → Xem chi tiết".

| Dấu hiệu trong SRS | Variant | DTO Files cần sinh | Service Return Type |
|---------------------|---------|--------------------|--------------------|
| Chi tiết hiển thị **form thông tin** (label-value pairs) | **A** | `DetailRequest` + `DetailResponse` | `{Name}DetailResponse` (single) |
| Chi tiết hiển thị **list records con** (đơn giản, ít data) | **B** | `DetailResponse` only | `List<{Name}DetailResponse>` |
| Chi tiết hiển thị **BẢNG DỮ LIỆU** (có STT, phân trang, cột header) | **C** | `HisFilterRequest` + `DetailResponse` (with `[ClientTableConfig]`) | `PaginationResponse<{Name}DetailResponse>` |

**Quy tắc nhận biết Variant C:**
- SRS mô tả bảng con có **phân trang** (Pagination), **STT**, **nhiều cột header**
- Detail Response cần **`[ClientTableConfig]`** attributes (để FE tự render cột)
- Service phải trả `PaginationResponse` kèm `ClientTableHelper.GetColumns<>()`
- Filter Request phải extends `BaseFilterRequest` (chứa `PageNumber`, `PageSize`)

**Tham khảo sub-skills:** `02-dto-gen.md` (Section 4: Detail Variant Decision Table), `05-controller-gen.md`, `04-service-gen.md`, `03-interface-gen.md`.

---

## 🎯 Selective Reading Rule

**Đọc sub-skill tương ứng với step đang thực thi!**

### CRUD Mode Steps

| File                   | Description                                  | When to Read                   |
| ---------------------- | -------------------------------------------- | ------------------------------ |
| `01-entity-gen.md`        | Sinh Entity class từ DDL/SRS                 | Step 1 — Entity Generation     |
| `06-dbset-gen.md`         | Đăng ký DbSet trong DbContext                | Step 2 — DbSet Registration    |
| `02-dto-gen.md`           | Sinh Request/Response DTO                    | Step 3 — DTO Generation        |
| `03-interface-gen.md`     | Sinh Interface + folder structure            | Step 4 — Interface Generation  |
| `04-service-gen.md`       | Sinh Service implementation (DI trực tiếp)   | Step 5 — Service Generation    |
| `05-controller-gen.md`    | Sinh Controller (DI, [Authorize], thin)      | Step 6 — Controller Generation |
| `07-functionconst-gen.md` | Sinh FunctionConst partial class             | Step 6 — Controller Generation |
| `09-self-scan.md`         | Verify code vừa sinh bằng convention checker | Step 7 — Self-Scan             |

### REPORT Mode Steps

| File                          | Description                                   | When to Read                    |
| ----------------------------- | --------------------------------------------- | ------------------------------- |
| `08-report-gen.md`               | Toàn bộ convention sinh Report module         | BẮT BUỘC đọc khi gen Report    |
| `05-controller-gen.md`           | Convention Controller (dùng chung)            | Step 5 — Controller (tham khảo) |
| `07-functionconst-gen.md`        | Convention FunctionConst (dùng chung)         | Step 6 — FunctionConst          |
| `09-self-scan.md`                | Verify code vừa sinh                          | Step 7 — Self-Scan              |

### Convention Reference (single source of truth)

| # | Convention File (trong `../dotnet-convention-checker/`) | Referenced by          | Mode  |
|---|----------------------------------------------------------|------------------------|-------|
| 01 | `01-entity-convention.md`                               | `01-entity-gen.md`     | CRUD  |
| 02 | `02-dto-convention.md`                                  | `02-dto-gen.md`        | CRUD  |
| 03 | `03-interface-convention.md`                            | `03-interface-gen.md`  | Both  |
| 04 | `04-service-convention.md`                              | `04-service-gen.md`    | Both  |
| 05 | `05-controller-convention.md`                           | `05-controller-gen.md` | Both  |
| 06 | `06-mapping-convention.md`                              | `04-service-gen.md`    | CRUD  |
| 07 | `07-relationship-convention.md`                         | `01-entity-gen.md`     | CRUD  |
| 08 | `08-functionconst-convention.md`                        | `07-functionconst-gen.md` | Both |
| 10 | `10-report-convention.md`                               | `08-report-gen.md`     | REPORT |

---

## ⚠️ Core Principle

- **Convention = Source of Truth:** KHÔNG duplicate rules — tham chiếu convention file.
- Mỗi step PHẢI ĐỌC convention file TRƯỚC, rồi mới sinh code.
- Code sinh ra PHẢI pass được convention checker (Self-Scan).
- Chạy từng Step **TUẦN TỰ** — output step trước là input step sau.
- Dùng **DI trực tiếp** (constructor injection) — TUYỆT ĐỐI KHÔNG dùng `IServiceProvider`.

---

## 📋 Step → Sub-Skill Mapping Table

### CRUD Mode (BẮT BUỘC TUÂN THỦ)

| Step  | Sub-Skill (ĐỌC FILE TRƯỚC KHI CHẠY)          | Output Location                                       |
| ----- | -------------------------------------------- | ----------------------------------------------------- |
| **1** | `01-entity-gen.md`                              | `BOBase.Domain/Entities/{Module}/`                    |
| **2** | `06-dbset-gen.md`                               | `BOBase.Infrastructure/Data/*DbContext.cs`            |
| **3** | `02-dto-gen.md`                                 | `modules/BOModule.{Name}/DTOs/`                       |
| **4** | `03-interface-gen.md`                           | `modules/BOModule.{Name}/Services/`                   |
| **5** | `04-service-gen.md`                             | `modules/BOModule.{Name}/Services/Impls/`             |
| **6** | `05-controller-gen.md` + `07-functionconst-gen.md` | `modules/BOModule.{Name}/Controllers/` + `Constants/` |
| **7** | `09-self-scan.md`                               | Trình bày chat hoặc file pipeline                     |

### REPORT Mode (BẮT BUỘC TUÂN THỦ)

| Step  | Sub-Skill (ĐỌC FILE TRƯỚC KHI CHẠY)          | Output Location                                                |
| ----- | -------------------------------------------- | -------------------------------------------------------------- |
| **1** | `08-report-gen.md` (Section: Response)          | `modules/BOModule.Report/DTOs/Omni/{Feature}/Responses/`       |
| **2** | `08-report-gen.md` (Section: Setting)           | `modules/BOModule.Report/Models/Settings/Omni/`                |
| **3** | `08-report-gen.md` (Section: Request)           | `modules/BOModule.Report/DTOs/Omni/{Feature}/Requests/`        |
| **4** | `08-report-gen.md` (Section: Interface)         | `modules/BOModule.Report/Services/Omni/`                       |
| **5** | `08-report-gen.md` (Section: Service)           | `modules/BOModule.Report/Services/Implements/omni/`            |
| **6** | `08-report-gen.md` (Section: Controller)        | `modules/BOModule.Report/Controllers/`                         |
| **7** | `07-functionconst-gen.md`                       | `modules/BOModule.Report/FunctionConst.cs` (append)            |
| **8** | EPackage enum (nếu cần)                      | `src/BOBase.Domain/Enums/EPackage.cs` (append)                 |
| **9** | `09-self-scan.md`                               | Trình bày chat hoặc file pipeline                              |

---

## Execution Protocol

### Input Requirements

| Mode   | Cần có                                          |
|--------|------------------------------------------------|
| CRUD   | DDL script + SRS/Confluence + Function IDs      |
| REPORT | Oracle Package file (.pks/.pkb) + Function IDs  |

### Trong từng Step:

1. **Đọc sub-skill file** → nắm rules, template, và convention reference.
2. **Đọc convention file tương ứng** (cross-reference) → nắm checklist.
3. **Sinh code** theo template trong sub-skill, tuân thủ convention.
4. **Self-check** code vừa sinh vs checklist convention.
5. **Ghi file** vào đúng location.

### Sau khi hoàn tất:

1. Chạy **Self-Scan** để verify.
2. Fix bất kỳ finding nào (CRITICAL/WARNING).
3. Trình bày kết quả cho User.

---

## Decision Checklist

### CRUD Mode
- [ ] Đã chạy đủ 7 Steps?
- [ ] Mỗi step đã đọc convention trước?
- [ ] DbSet đã được đăng ký trong DbContext?
- [ ] Code dùng DI trực tiếp (không IServiceProvider)?
- [ ] Self-scan trả kết quả 0 CRITICAL, 0 WARNING?
- [ ] Code nằm đúng folder location?

### REPORT Mode
- [ ] Đã đọc file .pks/.pkb?
- [ ] Response properties khớp outer SELECT columns?
- [ ] Request `GetParameters()` đúng thứ tự procedure signature?
- [ ] Setting `SetStoreProcedue()` đúng tên procedure?
- [ ] EPackage enum value đã thêm?
- [ ] FunctionConst đã append?
- [ ] Self-scan trả kết quả 0 CRITICAL, 0 WARNING?

---

## Anti-Patterns

❌ Sinh code mà không đọc convention trước.
❌ Copy-paste convention rules vào sub-skill (duplicate).
❌ Dùng `IServiceProvider` trong code sinh ra.
❌ Bỏ qua Self-Scan.
❌ Sinh code không đúng folder structure.
❌ Dùng CRUD template cho Report hoặc ngược lại.
