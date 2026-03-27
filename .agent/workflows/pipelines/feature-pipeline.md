---
description: Master pipeline — auto-detect progress, run next steps. Supports --stop-at and --start-from for partial execution.
---

# /feature-pipeline — Master Feature Pipeline

$ARGUMENTS

---

## Task

Orchestrate the entire modular feature pipeline. **User chỉ cần nhớ 1 lệnh này.**

Agent tự phát hiện tiến trình hiện tại từ `openspec/changes/<name>/` và chạy step tiếp theo.

---

## Syntax

```
/feature-pipeline <name>                                        # Auto-detect và chạy tiếp
/feature-pipeline <name> --stop-at <step>                       # Từ init → đến step rồi dừng
/feature-pipeline <name> --start-from <step>                    # Từ step → đến hết nhóm
/feature-pipeline <name> --start-from <step> --stop-at <step>   # Từ step A → đến step B
/feature-pipeline <name> --only <step>                          # Chỉ chạy 1 step duy nhất
```

### Step Map

```
1.init → 2.srs → 3.specs → 4.design → 5.tasks ─┬→ 5a.oracle        (parallel)
                                                  ├→ 5b.dotnet        (parallel)
                                                  ├→ 5c.angular       (parallel)
                                                  ├→ 5d.oracle-crud   (parallel, optional)
                                                  └→ 5e.oracle-report (parallel, optional)
                                                         ↓
                                               6.security (optional)
                                                         ↓
                                               7.review → 8.archive
```

| Step | Name (dùng trong flags) | Skill/Workflow | Output | Depends on |
|------|------------------------|----------------|--------|------------|
| 1 | `init` | `init` (step) | `metadata.yaml` + `proposal.md` | — |
| 2 | `srs` | `srs` (step) | `srs.md` | init |
| 3 | `specs` | `specs` (step) | `specs/*.md` | srs |
| 4 | `design` | `design` (step) | `design.md` | specs |
| 5 | `tasks` | `tasks` (step) | `tasks.md` | design |
| 5a | `oracle` | `oracle-table-gen` | `scripts/*.sql` | design |
| 5b | `dotnet` | `dotnet-gen` | `.cs` source code | tasks |
| 5c | `angular` | `angular-gen` | `.ts` source code | tasks |
| 5d | `oracle-crud` | `oracle-crud-gen` | `scripts/*.pck` | design + scripts/*.sql |
| 5e | `oracle-report` | `oracle-report-gen` | `scripts/*.pck` | design + scripts/*.sql |
| 6 | `security` | `security-scan` | `security-report.md` | code files generated |
| 7 | `review` | `review` (step) | `todo-uncover.md`, `new-apis.md` | branches done |
| 8 | `archive` | `archive` (step) | Archived change | review |

> **5a–5e là parallel branches** — không phụ thuộc lẫn nhau. `--stop-at dotnet` sẽ KHÔNG chạy oracle.
> **5d/5e là optional** — chỉ chạy khi user yêu cầu (`--only oracle-crud`) hoặc design.md có PL/SQL package specs.
> **6.security là optional** — chỉ chạy khi user yêu cầu.

---

## Auto-Detection Logic

Scan `openspec/changes/<name>/` và xác định step hiện tại:

```
IF change directory không tồn tại     → Step 1 (init)
IF metadata.yaml missing              → Step 1 (init)
IF proposal.md missing                → Step 1 (init)
IF srs.md missing                     → Step 2 (srs)
IF specs/ empty or missing            → Step 3 (specs)
IF design.md missing                  → Step 4 (design)
IF tasks.md missing                   → Step 5 (tasks)
IF tasks.md exists AND có chưa-hoàn-thành:
  - scripts/ empty                    → 5a (oracle)
  - unchecked backend tasks           → 5b (dotnet)
  - unchecked frontend tasks          → 5c (angular)
  - design.md has PL/SQL CRUD specs   → 5d (oracle-crud)
  - design.md has PL/SQL report specs → 5e (oracle-report)
  (chạy tất cả applicable branches)
IF security-report.md missing AND user requested → Step 6 (security)
IF todo-uncover.md missing            → Step 7 (review)
ELSE                                  → Step 8 (archive)
```

---

## Execution Rules

### Default (no flags)

Auto-detect current step → chạy step đó + các step kế tiếp **cho đến hết nhóm hiện tại**:

| Nhóm | Steps | Lý do gộp |
|------|-------|-----------|
| **Planning** | init + srs | Nhẹ, cùng context |
| **Specs** | specs + design | Specs → Design liên tục |
| **Implementation** | tasks + oracle + dotnet + angular | Gen tasks rồi gen code (branches chạy tuần tự trong cùng conversation) |
| **Finalize** | review + archive | Review rồi archive |

Sau khi hoàn thành nhóm → dừng, hướng dẫn user mở conversation mới gọi lại.

### `--stop-at <step>`

**Luôn bắt đầu từ init** (hoặc từ auto-detected step) → chạy đến step chỉ định rồi **DỪNG**.

**⚠️ oracle/dotnet/angular là parallel branches — `--stop-at` chỉ chạy branch được chỉ định:**

```
/feature-pipeline add-customer --stop-at design
→ Chạy: init → srs → specs → design → DỪNG

/feature-pipeline add-customer --stop-at dotnet
→ Chạy: init → srs → specs → design → tasks → dotnet → DỪNG
→ KHÔNG chạy oracle, angular (parallel branches khác)

/feature-pipeline add-customer --stop-at oracle
→ Chạy: init → srs → specs → design → tasks → oracle → DỪNG
→ KHÔNG chạy dotnet, angular
```

### `--start-from <step>`

Bỏ qua auto-detect, **bắt đầu từ step chỉ định** → chạy đến hết nhóm. Prerequisite files PHẢI tồn tại.

```
/feature-pipeline add-customer --start-from angular
→ Bỏ qua: init, srs, specs, design, tasks, oracle, dotnet
→ Chạy: angular → DỪNG (hết nhóm Implementation)
→ Prerequisite: tasks.md phải tồn tại
```

### `--start-from <step> --stop-at <step>` (kết hợp)

Chạy từ step A → đến step B rồi DỪNG. Cả 2 flags hoạt động cùng lúc.

```
/feature-pipeline add-customer --start-from design --stop-at dotnet
→ Chạy: design → tasks → oracle → dotnet → DỪNG
→ Prerequisite: srs.md + specs/ phải tồn tại

/feature-pipeline add-customer --start-from tasks --stop-at oracle
→ Chạy: tasks → oracle → DỪNG
```

### `--only <step>`

Chạy **đúng 1 step**, không chạy step kế tiếp. Tương đương `--start-from X --stop-at X`.

```
/feature-pipeline add-customer --only oracle
→ Chỉ chạy: oracle-table-gen → DỪNG
```

---

## Prerequisite Check

Trước khi chạy bất kỳ step nào, verify prerequisites:

| Step | Requires |
|------|----------|
| `srs` | `proposal.md` |
| `specs` | `proposal.md`, `srs.md` |
| `design` | `proposal.md`, `srs.md`, `specs/` not empty |
| `tasks` | `design.md`, `specs/` not empty |
| `oracle` | `design.md` (Section: Entity Design) |
| `dotnet` | `tasks.md` (backend tasks exist) |
| `angular` | `tasks.md` (frontend tasks exist) |
| `oracle-crud` | `design.md` + `scripts/*.sql` (DDL phải tồn tại) |
| `oracle-report` | `design.md` + `scripts/*.sql` (DDL phải tồn tại) |
| `security` | Code files generated (có .cs hoặc .ts) |
| `review` | Code files generated |
| `archive` | `todo-uncover.md` exists |

Nếu prerequisite thiếu → **HALT** và hướng dẫn user chạy step trước.

---

## Step Execution

Mỗi step thực chất là **delegate** sang skill/workflow tương ứng:

```
Step "init"          → Execute step init
Step "srs"           → Execute step srs
Step "specs"         → Execute step specs
Step "design"        → Execute step design
Step "tasks"         → Execute step tasks
Step "oracle"        → Execute standalone oracle-table from change <name>
Step "dotnet"        → Execute standalone dotnet-gen from change <name>
Step "angular"       → Execute standalone angular-gen from change <name>
Step "oracle-crud"   → Execute standalone oracle-crud from change <name>
Step "oracle-report" → Execute standalone oracle-report from change <name>
Step "security"      → Execute standalone security-scan on change <name> generated code
Step "review"        → Execute step review
Step "archive"       → Execute step archive
```

---

## After Each Group

Khi hoàn thành nhóm hiện tại, hiển thị:

```
✅ Completed: [list of steps done]
📁 Change: openspec/changes/<name>/
📊 Progress: [X/10 steps]

👉 Next: Mở conversation mới, chạy:
   /feature-pipeline <name>
```

---

## Usage Examples

```
# Bắt đầu feature mới (auto: init + srs)
/feature-pipeline add-customer

# Tiếp tục (auto-detect: specs + design)
/feature-pipeline add-customer

# Chạy từ đầu đến design rồi dừng
/feature-pipeline add-customer --stop-at design

# Chạy từ đầu đến hết code gen
/feature-pipeline add-customer --stop-at angular

# Chạy từ design đến dotnet
/feature-pipeline add-customer --start-from design --stop-at dotnet

# Chỉ sinh DDL Oracle
/feature-pipeline add-customer --only oracle

# Sinh code từ Angular trở đi
/feature-pipeline add-customer --start-from angular

# Chỉ chạy dotnet code gen
/feature-pipeline add-customer --only dotnet
```

---

## Before Starting

Nếu `$ARGUMENTS` không có change name, hỏi:
> "Tên feature là gì? (kebab-case, ví dụ: `add-customer-management`)"
