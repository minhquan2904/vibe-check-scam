# 🚀 Modular Feature Pipeline — Hướng Dẫn Sử Dụng

> **Bạn chỉ cần nhớ 1 lệnh: `/feature-pipeline <tên-feature>`**
>
> Agent sẽ tự biết phải làm gì tiếp theo.

---

## Mục Lục

1. [Quick Start — 3 phút](#1-quick-start)
2. [Pipeline hoạt động thế nào?](#2-pipeline-hoat-dong-the-nao)
3. [Các lệnh thường dùng](#3-cac-lenh-thuong-dung)
4. [Chạy từng phần riêng lẻ](#4-chay-tung-phan-rieng-le)
5. [Cấu trúc thư mục output](#5-cau-truc-thu-muc-output)
6. [FAQ](#6-faq)

---

## 1. Quick Start

### Bước 1 — Mở conversation mới, gõ:

```
/feature-pipeline add-customer-management
```

Agent sẽ:
- Hỏi bạn mô tả feature (hoặc đưa link Confluence/Jira)
- Tự sinh `proposal.md` + `srs.md`
- Báo: *"Done. Mở conversation mới, gọi lại /feature-pipeline add-customer-management"*

### Bước 2 — Mở conversation mới, gõ lại:

```
/feature-pipeline add-customer-management
```

Agent tự detect tiến trình → chạy tiếp (specs + design).

### Bước 3 — Lặp lại cho đến khi hoàn thành.

**Tổng cộng ~4 conversations** cho 1 feature hoàn chỉnh (planning → code → review).

---

## 2. Pipeline Hoạt Động Thế Nào?

### Luồng tổng thể

```
📋 Planning        📐 Specs           🔨 Implementation          🔒 Scan     ✅ Finalize
─────────────      ─────────────     ──────────────────────      ────────    ─────────────
init + srs    →    specs + design →  tasks ─┬→ oracle            security →  review + archive
                                            ├→ dotnet
                                            ├→ angular
                                            ├→ oracle-crud  (opt)
                                            └→ oracle-report(opt)
```

### Steps chi tiết

| # | Step | Làm gì | Output |
|---|------|--------|--------|
| 1 | `init` | Thu thập yêu cầu, tạo proposal | `metadata.yaml`, `proposal.md` |
| 2 | `srs` | Sinh tài liệu đặc tả chi tiết | `srs.md` |
| 3 | `specs` | Sinh behavioral specs (scenarios) | `specs/*.md` |
| 4 | `design` | Thiết kế kiến trúc, API, Entity | `design.md` |
| 5 | `tasks` | Tạo danh sách tasks theo layer | `tasks.md` |
| 5a | `oracle` | Sinh DDL Oracle scripts | `scripts/*.sql` |
| 5b | `dotnet` | Sinh code .NET backend | `.cs` files |
| 5c | `angular` | Sinh code Angular frontend | `.ts` files |
| 5d | `oracle-crud` | Sinh PL/SQL CRUD package *(optional)* | `scripts/*.pck` |
| 5e | `oracle-report` | Sinh PL/SQL report package *(optional)* | `scripts/*.pck` |
| 6 | `security` | Security scan *(optional)* | `security-report.md` |
| 7 | `review` | Kiểm tra code đã sinh | `todo-uncover.md`, `new-apis.md` |
| 8 | `archive` | Đóng gói, lưu trữ | Archived change |

### Auto-detect: Agent tự biết chạy gì

Mỗi khi bạn gọi `/feature-pipeline <name>`, agent scan thư mục `openspec/changes/<name>/`:

```
Không có gì          → Chạy init + srs
Có proposal + srs    → Chạy specs + design
Có specs + design    → Chạy tasks + oracle + dotnet + angular
Có code              → Chạy review + archive
```

**Bạn không cần nhớ thứ tự!**

---

## 3. Các Lệnh Thường Dùng

### Chạy tự động (recommended)

```bash
/feature-pipeline <name>
```

### Dừng tại step cụ thể (luôn bắt đầu từ init)

```bash
# Từ đầu → đến design rồi dừng
/feature-pipeline <name> --stop-at design

# Từ đầu → đến hết code gen
/feature-pipeline <name> --stop-at angular

# Từ đầu → đến tasks rồi dừng
/feature-pipeline <name> --stop-at tasks
```

### Bắt đầu từ step cụ thể

```bash
# Từ Angular trở đi (cần tasks.md đã có)
/feature-pipeline <name> --start-from angular

# Từ .NET backend trở đi
/feature-pipeline <name> --start-from dotnet

# Từ review trở đi (cần code đã sinh)
/feature-pipeline <name> --start-from review
```

### Kết hợp: từ step A → đến step B

```bash
# Từ design → đến dotnet
/feature-pipeline <name> --start-from design --stop-at dotnet

# Từ tasks → đến oracle
/feature-pipeline <name> --start-from tasks --stop-at oracle
```

### Chạy đúng 1 step

```bash
# Chỉ gen DDL Oracle
/feature-pipeline <name> --only oracle

# Chỉ gen Angular
/feature-pipeline <name> --only angular

# Chỉ gen .NET
/feature-pipeline <name> --only dotnet
```

---

## 4. Chạy Từng Phần Riêng Lẻ

Ngoài master pipeline, bạn có thể chạy **workflows standalone** (không cần pipeline):

### Gen Oracle DDL từ Jira/Confluence

```bash
/oracle-table-gen from Jira BO-1023
/oracle-table-gen from confluence "https://..."
```

### Gen .NET code từ DDL

```bash
/dotnet-gen from DDL (paste CREATE TABLE script)
/dotnet-gen module Background
```

### Gen Angular code từ Swagger

```bash
/angular-gen from swagger ./swagger.json endpoint /api/city
/angular-gen component-only city
```

> **Tip:** Các workflow standalone không liên kết pipeline — dùng khi cần gen code nhanh mà không cần full feature flow.

---

## 5. Cấu Trúc Thư Mục Output

Sau khi pipeline chạy xong, output nằm trong:

```
openspec/changes/<name>/
├── .openspec.yaml            ← OpenSpec CLI metadata
├── metadata.yaml             ← Feature metadata (ID, type, summary)
├── proposal.md               ← Proposal (what & why)
├── srs.md                    ← Software Requirements Spec
├── design.md                 ← Technical Design
├── tasks.md                  ← Implementation tasks [x]
├── specs/
│   ├── requirement-1.md      ← Behavioral specs
│   └── requirement-2.md
├── scripts/
│   └── create_table.sql      ← Oracle DDL scripts
├── todo-uncover.md           ← Uncovered items (post-review)
├── new-apis.md               ← New APIs documented
└── delta-spec.md             ← Spec changes (maintenance)
```

---

## 6. FAQ

### Q: Tại sao phải chạy nhiều conversations?

**A:** Để tránh context window overflow. Mỗi conversation xử lý 2-4 steps nhẹ → agent luôn hiểu đúng context → code không bị sai.

### Q: Nếu tôi muốn sửa design sau khi đã gen code?

**A:**
```bash
# Sửa design.md bằng tay hoặc nhờ agent
# Rồi gen lại code:
/feature-pipeline <name> --start-from dotnet
```

### Q: Nếu chỉ cần backend, không cần Angular?

**A:**
```bash
/feature-pipeline <name> --stop-at dotnet
```

### Q: Tôi có thể chạy song song 2 features không?

**A:** Được. Mỗi feature có thư mục riêng trong `openspec/changes/`:
```bash
# Conversation 1
/feature-pipeline add-customer

# Conversation 2 (parallel)
/feature-pipeline add-report-lfx
```

### Q: Workflow nào bắt buộc đọc rules/skills?

**A:** `srs`, `specs`, `design`, `apply` (dotnet/angular) — agent sẽ **HALT** nếu không tìm thấy required rules/skills. Danh sách rules nằm trong `openspec/mapping/artifact_context_modular.yml`.

---

## Tech Stack

Pipeline này sinh code cho:

| Layer | Tech |
|-------|------|
| Backend | C# / .NET 8 / Entity Framework Core |
| Frontend | Angular 17 / TypeScript |
| Database | Oracle (native DDL) |
| Cache | Redis |
