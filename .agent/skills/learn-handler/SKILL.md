---
name: learn-handler
description: Learn handler/business-logic patterns — scan CQRS handlers, command/query routing, lifecycle hooks (preHandle/aroundHandle/postHandle), and document the full handler DNA for LLM reuse.
---

## Purpose

Automatically scan the source code, extract and document the **Handler Layer DNA** so the LLM can:
1. Understand how many Handler types exist (Command, Query, ETag, Transaction, ...)
2. Know which base class to use for each type, lifecycle hooks, and generic type order
3. Know how request → handler routing works (CQRS MessageBus? Direct injection?)
4. Create new handlers with the correct base class, override methods, and injection pattern

**Output filename**: `knowledge_handler.md`
**Output directory**: Determined by caller. Default: `base_knowledge/structures/propose/`.

---

## Input

- **Project root** (required): project root directory
- **Scope** (optional): specific service, defaults to scan ALL

---

## Step 1 — Find Entry Points

Do NOT assume class names or CQRS framework. **Scan the actual codebase**.

### 1a. Find all Base Handler Classes

```
grep_search: "abstract class Base.*Handler" in {project_root}
grep_search: "class Base.*Handler" in {project_root}
grep_search: "BaseCommandHandler" in {project_root}
grep_search: "BaseQueryHandler" in {project_root}
grep_search: "BaseGetDataByETag" in {project_root}
```

**Determine**:
- How many base handler types exist? (Command? Query? ETag? Transaction Init/Confirm?)
- For each base class: package, generic params, abstract methods, optional overrides
- Hierarchy: BaseCommandHandler → BaseInitFinancialHandler → Concrete?

### 1b. Find CQRS / MessageBus Routing

```
grep_search: "MessageBus" in {project_root}
grep_search: "IMessageBus" in {project_root}
grep_search: "cqrs" in {project_root}
```

**Determine**:
- How are handlers registered with MessageBus? (annotation? generic type matching?)
- Request class type → Handler mapping rule
- Does the handler class have `@Component`?

### 1c. Find all Concrete Handler Files

```
find_by_name: Pattern="*Handler.java", SearchDirectory={project_root}
```

Exclude: `ExceptionHandler`, `GlobalExceptionHandler`, `GrpcGlobalExceptionHandler`.

For EACH handler found, read the file and record:
- Class name + service/module it belongs to
- Base class extended (and generic types)
- Constructor dependencies (factories/clients injected)
- Overridden methods: list all `@Override`
- Request class + Response class

### 1d. Classify Handlers

Group all handlers found by base class:

| Base Class | Handler Count | Services | Purpose |
|---|---|---|---|
| BaseCommandHandler | ? | ? | Generic command handler |
| BaseGetDataByETagHandler | ? | ? | ETag-based data retrieval |
| BaseInitFinancialHandler | ? | ? | Financial init phase |
| ... | | | |

---

## Step 2 — Analyze Base Handler Internals

For EACH base handler class, read the full source and document:

### Per base class:

#### 2a. Lifecycle Hooks
- **preHandle(request)**: Purpose? Default behavior? Subclass override pattern?
- **aroundHandle(request)**: Purpose? Abstract or optional?
- **postHandle(request, response)**: Purpose? Cleanup?
- **onError(request, exception)**: Error handling hook?

Draw lifecycle:
```
Request → preHandle() → aroundHandle() → postHandle() → Response
                                              ↓ (on error)
                                         onError()
```

#### 2b. Generic Type Parameters
- Exact order: `<Request, Response>` or `<ID, Model, Request, Response>`?
- Constraint: `T extends BaseRequest`?

#### 2c. Constructor Pattern
- Injection pattern: Direct? ObjectProvider? @Qualifier?
- Common dependencies: factories, clients, configs

#### 2d. Request → Handler Routing
- MessageBus dispatches based on Request class type
- Each Request class maps to EXACTLY 1 Handler
- Handler MUST have `@Component` annotation

---

## Step 3 — Analyze Sample Handlers

Select at least **5 representative handlers** (1 per base type if available), read fully and document:

Per sample:
- Controller calls `execute(request)` → MessageBus routes → Handler
- preHandle logic (validation, load data)
- aroundHandle logic (business, build response)
- Factories/Clients injected and where they are used
- Response building pattern

---

## Step 4 — Write `knowledge_handler.md`

> ⚠️ ALL class names MUST be REAL values from source — no placeholders.

### Output template:

````markdown
# Handler Knowledge

_Generated from codebase analysis — YYYY-MM-DD._

---

## Handler Type Comparison
| Handler Type | Base Class | Generic Types | Lifecycle | Use Case |
|---|---|---|---|---|
| {Type 1} | {class} | {types} | {hooks} | {when to use} |

## Routing Mechanism
{CQRS MessageBus? Annotation? Direct call? Detailed description}

## Decision Tree
```
┌─ What does the handler need to do?
│
├── Query data with ETag?
│   └── extends {ETag Handler}
├── Command that changes state?
│   └── extends {Command Handler}
├── Transaction Init?
│   └── extends {Init Handler} (Financial / NonFinancial)
├── Transaction Confirm?
│   └── extends {Confirm Handler}
```

---

## Type 1: {Base handler name}

## Type 1: {Base handler name}

### 1. Dùng để làm gì? (Purpose)
{Định nghĩa rõ base handler này giải quyết bài toán gì trong hệ thống CQRS/Architecture}

### 2. Khi nào dùng? (When to use)
{Liệt kê các Use-case cụ thể, ví dụ: "Dùng khi cần thay đổi trạng thái database", "Dùng khi khởi tạo giao dịch tài chính", "Dùng khi lấy dữ liệu có cache ETag"}

### 3. Dùng như thế nào? (How to use)

#### 3.1. Các phương thức bắt buộc (Must Override) & Tuỳ chọn
| Method | Type | Signature | Purpose |
|---|---|---|---|

#### 3.2. Vòng đời thực thi (Lifecycle Flow)
{Ví dụ: preHandle → aroundHandle → postHandle}

#### 3.3. Pattern Inject Dependencies (Constructor)
{REAL code snippet chứng minh cách inject factory/client vào handler này}

#### 3.4. Code Template chuẩn
{Template code hoàn chỉnh, copy-paste được, sử dụng đúng thứ tự Generic Types và Annotations}

#### 3.5. Ví dụ thực tế (Real Examples)
{Table: Handler | Service | Request | Response | Key Overrides}

---

## Type 2: {...}
{Same format}

---

## Handler Registration Rules
{@Component, Request type uniqueness, MessageBus dispatch}

## Code Template
{Per type — ready-to-copy template using real class names}
````

---

## Step 5 — Self-Verification

1. [ ] ALL handler types identified and documented with Q&A
2. [ ] Routing mechanism documented (MessageBus/CQRS)
3. [ ] Lifecycle hooks documented per base class
4. [ ] Decision tree present
5. [ ] ALL concrete handlers listed in tables
6. [ ] NO `{placeholder}` remains
7. [ ] Code snippets from actual source
8. [ ] Constructor patterns per type documented
9. [ ] Generic type order documented per type

---

## Guardrails

- DO NOT assume handler names — find actual names from source
- DO NOT invent handler instances — only document what FOUND
- Exclude ExceptionHandler, GrpcExceptionHandler from listing
- Cross-reference `knowledge_architecture.md` for overall layer context
- Cross-reference `knowledge_transaction_flow.md` for Init/Confirm handlers
