---
name: learn-transaction-flow
description: Learn all financial and non-financial transaction flows — scan Init/Auth/Confirm handlers, document the full lifecycle, base classes, overridable methods, and when to use each flow type.
---

## Purpose

Automatically scan the source code, extract and document the **Transaction Flow DNA** so the LLM can:
1. Understand how many transaction flow types exist (financial, non-financial, hybrid, ...)
2. Know which base handler to use for each flow, and which methods to override
3. Understand the full lifecycle: Init → (Auth) → Confirm + revert chain
4. Create new transaction features following the correct pattern

**Output filename**: `knowledge_transaction_flow.md`
**Output directory**: Determined by caller. Default: `base_knowledge/structures/propose/`.

---

## Input

- **Project root** (required): project root directory
- **Scope** (optional): specific service, defaults to scan ALL

---

## Step 1 — Find Entry Points

Do NOT assume class names. Transaction flows may use different naming: Financial/NonFinancial, Payment/Transfer, etc. **Scan the actual codebase**.

### 1a. Find Base Handler Classes (Init/Auth/Confirm)

This is the most critical step — find the abstract base classes for transactions.

```
grep_search: "abstract class Base.*Init" in {project_root}
grep_search: "abstract class Base.*Confirm" in {project_root}
grep_search: "abstract class Base.*Auth" in {project_root}
grep_search: "abstract class Base.*Transaction.*Handler" in {project_root}
grep_search: "abstract class Base.*Payment.*Handler" in {project_root}
grep_search: "abstract class Base.*Transfer.*Handler" in {project_root}
```

**Determine**:
- How many base handler types exist? (e.g., Init Financial, Init NonFinancial, Init Method, Confirm Financial, Confirm NonFinancial)
- Real name of each base class (may NOT be "Financial"/"NonFinancial")
- Package location (usually in `common/` or a shared module)

### 1b. Find all Transaction Handlers (concrete)

From the base classes found in 1a, search for extends:

```
grep_search: "extends {BaseInitClass}" in {project_root}    # for each base init found
grep_search: "extends {BaseConfirmClass}" in {project_root}  # for each base confirm found
grep_search: "extends {BaseAuthClass}" in {project_root}    # for each base auth found
```

For EACH handler found, read the file and record:
- Class name + service it belongs to
- Generic types: `<Request, Response>`
- Base class extended
- Constructor dependencies (factories/services injected)
- Overridden methods: list all `@Override`
- TransactionType returned (if there is a `getTransactionType()` method)

### 1c. Find Transaction Type Enums

```
grep_search: "implements ITransactionType" in {project_root}
grep_search: "implements.*TransactionType" in {project_root}
grep_search: "enum.*TransactionType" in {project_root}
grep_search: "enum.*TransferType" in {project_root}
grep_search: "enum.*PaymentType" in {project_root}
```

For EACH enum found, read and record:
- Enum name + values
- Config methods: `requireAuthMethod()`, max amount, allowed channels
- Confirm executor: `getConfirmExecutor()` → which executor handles confirm

### 1d. Find Confirm Executors / Confirm Strategies

```
grep_search: "implements ITransactionConfirmExecutor" in {project_root}
grep_search: "implements.*ConfirmExecutor" in {project_root}
grep_search: "implements ITransferConfirm" in {project_root}
grep_search: "implements.*ConfirmStrategy" in {project_root}
```

For EACH executor/strategy found:
- Class name
- Confirm logic: authorization, payment, revert chain
- Revert handling pattern

### 1e. Find Auth Method Types

```
grep_search: "enum.*AuthMethod" in {project_root}
grep_search: "enum.*OtpMethod" in {project_root}
grep_search: "MainAuthMethod\|SubAuthMethod\|VerificationMethod" in {project_root}
```

### 1f. Find Transaction Request/Response Models

```
grep_search: "extends Base.*Init.*Request" in {project_root}
grep_search: "extends Base.*Confirm.*Request" in {project_root}
grep_search: "extends Base.*Transfer.*Request" in {project_root}
```

---

## Step 2 — Analyze Base Handler Internals

For EACH base handler class found in Step 1a:

### 2a. Read full source

Record:
- **Chain pattern** (if present): request chain → response chain
- **preHandle()**: validations, load data
- **aroundHandle()**: core logic, build transaction, auth
- **postHandle()**: cleanup, update
- **Abstract methods** subclass MUST implement
- **Optional overrides** subclass CAN override
- **Error handling**: exception types, error codes

### 2b. Determine lifecycle

For each flow type, draw the lifecycle:
```
[Client] → [Init Handler] → [Auth/OTP Handler] → [Confirm Handler] → [Executor/Strategy]
                ↓                    ↓                    ↓                    ↓
           create transaction   generate OTP        verify OTP          execute + revert
```

### 2c. Compare flow types

| Aspect | Flow Type 1 | Flow Type 2 | ... |
|---|---|---|---|
| Steps | ? | ? | |
| Auth required? | ? | ? | |
| Amount | ? | ? | |
| Base Init | ? | ? | |
| Base Confirm | ? | ? | |
| Init Request | ? | ? | |
| Confirm Request | ? | ? | |

---

## Step 3 — Map Full Transaction Flows

For EACH service, summarize all flows:

```
{ServiceName}:
  - Flow: {description}
    Init: {InitHandler} (extends {BaseHandler})
    Auth: {AuthHandler} (or BY_PASS)
    Confirm: {ConfirmHandler} (extends {BaseHandler})
    TransactionType: {enum value}
    Executor: {class that executes confirm}
```

---

## Step 4 — Write `knowledge_transaction_flow.md`

> ⚠️ ALL class names MUST be REAL values from source — no placeholders.

### Output template:

````markdown
# Transaction Flow Knowledge

_Generated from codebase analysis — YYYY-MM-DD._

---

## Flow Type Comparison

### Overview
{Table comparing ALL flow types found — from Step 2c}

### Decision Tree
{When to use which flow? Based on actual findings}

```
┌─ What type of feature?
│
├── Involves money (amount > 0)?
│   └── Flow Type: {financial flow name}
│
├── Does not involve money?
│   └── Flow Type: {non-financial flow name}
│
└── Other?
    └── Flow Type: {other flow name if present}
```

---

## Flow Type 1: {Real name — e.g., Financial / Payment / Transfer}

### 1. Dùng để làm gì? (Purpose)
{Mô tả ngắn gọn mục đích của luồng giao dịch này. Ví dụ: Chuyển tiền, Thanh toán hoá đơn, Mở tài khoản... có liên quan tới tài chính hay không?}

### 2. Khi nào dùng? (When to use)
{Nêu rõ các Use-case cụ thể BẮT BUỘC dùng luồng này. Ví dụ: Dùng khi có luồng cắt tiền, bắt buộc đi qua Auth và Confirm}

### 3. Dùng như thế nào? (How to use)

#### 3.1. Sơ đồ thực thi (Lifecycle Diagram)
{ASCII diagram với CÁC CLASS CÓ THẬT: Init → Auth → Confirm}

#### 3.2. Cấu hình Handler (Base Classes & Methods)

**Init Handler: {BaseClassName}**
- Bắt buộc implements (Must Override): {list}
- Cấu hình Constructor / Generic Types: {pattern}

**Auth Handler: {BaseClassName}**
- (Nếu có) Các method quan trọng: {Logic sinh OTP}

**Confirm Handler: {BaseClassName}**
- Bắt buộc implements (Must Override): {list}
- Xử lý Revert (Revert chain): {cơ chế roll-back khi lỗi}

#### 3.3. Các thông số Cấu hình (Configuration)
- Khai báo `TransactionType`: {Enum config}
- Cơ chế Error Handling: {Cách ném lỗi và map mã lỗi}

### All Flows Found
{Table: Service | Init Handler | Auth Handler | Confirm Handler | TransactionType | Description}

---

## Flow Type 2: {Real name}
{Same format as Flow Type 1}

---

## Transaction Type Registry
{ALL ITransactionType enums with config}

## Auth Methods
{ALL auth method enums/types found}

## Confirm Executors / Strategies
{ALL executor/strategy classes}

## Common Services Used
{Services/Factories commonly injected in handlers}

## Code Templates
{Template per flow type — using real class names}
````

---

## Step 5 — Self-Verification

1. [ ] ALL flow types identified and documented
2. [ ] ALL Init/Auth/Confirm handlers listed
3. [ ] Comparison table present with real data
4. [ ] Decision tree present
5. [ ] ALL TransactionType values listed
6. [ ] ALL AuthMethod types listed
7. [ ] Code templates use real class names
8. [ ] NO `{placeholder}` remains
9. [ ] Base class methods documented with signature + purpose
10. [ ] Revert chain pattern documented (if present)

---

## Guardrails

- DO NOT assume flow names are "Financial"/"NonFinancial" — find actual names
- DO NOT invent handler names — only document what FOUND
- DO NOT leave any `{placeholder}` in output
- Cross-reference `knowledge_architecture.md` for handler hierarchy
- Code snippets from actual source files
- If the project only has 1 flow type → document it clearly, do not invent a second one
