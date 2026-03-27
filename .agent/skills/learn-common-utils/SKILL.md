---
name: learn-common-utils
description: Learn all common utility classes, shared services, and helper methods available in the codebase — scan, catalog, and document to prevent duplicate implementations and promote reuse.
---

## Purpose

Automatically scan the source code, extract and document the **Common Utils DNA** so the LLM can:
1. Know which utilities already exist before writing new code
2. Avoid duplicate implementations for common logic (date/string/number/json/...)
3. Know which shared services can be injected (logging, exception, validation, ...)
4. Quickly find the right utility instead of writing from scratch

**Output filename**: `knowledge_common_utils.md`
**Output directory**: Determined by caller. Default: `base_knowledge/structures/propose/`.

---

## Input

- **Project root** (required): project root directory
- **Scope** (optional): defaults to scan `common/` module + shared libraries

---

## Step 1 — Find Utility Class Entry Points

### 1a. Find Util/Utils/Helper/Constant files

```
find_by_name: Pattern="*Util.java", SearchDirectory={project_root}
find_by_name: Pattern="*Utils.java", SearchDirectory={project_root}
find_by_name: Pattern="*Helper.java", SearchDirectory={project_root}
find_by_name: Pattern="*Constant*.java", SearchDirectory={project_root}
find_by_name: Pattern="*Constants.java", SearchDirectory={project_root}
```

### 1b. Find in common module

```
list_dir: {project_root}/common/
```

Recursively scan to find all packages containing `util`, `helper`, `constant`, `service`, `config`:
```
find_by_name: Pattern="*.java", SearchDirectory={project_root}/common/
```

### 1c. Find Shared Services

```
grep_search: "@Service" in {project_root}/common
grep_search: "@Component" in {project_root}/common
grep_search: "@Configuration" in {project_root}/common
```

### 1d. Find Exception Classes

```
find_by_name: Pattern="*Exception.java", SearchDirectory={project_root}
find_by_name: Pattern="*Error.java", SearchDirectory={project_root}
find_by_name: Pattern="*ErrorCode.java", SearchDirectory={project_root}
```

---

## Step 2 — Read and Classify

For EACH util/helper file found, read the source and document:

### Per file:
- **Class name** + **package** + **static or instance?**
- **Public methods**: for each method record:
  - Signature (params + return type)
  - 1-line description of functionality
  - Static? Instance?
- **Classify by domain**:
  - Date/Time utils
  - String utils
  - Number/Currency utils
  - JSON/Serialization utils
  - Collection utils
  - Security/Crypto utils
  - Validation utils
  - Logging utils
  - Exception utils
  - Http/Network utils
  - Other

### For Constants/Enums:
- **Class name** + **package**
- **Constant values**: name, type, value
- **Group**: Error codes? Config keys? Status? Channel?

### For Shared Services:
- **Class name** + **package**
- **Injection method**: `@Autowired`? Constructor? Interface?
- **Public methods**: signature + purpose

---

## Step 3 — Group & Catalog

Consolidate all utilities found into tables:

### By Category:
| Category | Classes | Key Methods |
|---|---|---|
| DateTime | TimeUtil, DateUtil, ... | toStringDate(), parse(), ... |
| String | StringUtil, ... | isEmpty(), format(), ... |
| Currency | CurrencyUtil, ... | roundAmount(), format(), ... |
| JSON | JsonUtil, ... | toJson(), fromJson(), ... |
| Security | SecurityUtil, ... | encrypt(), decrypt(), hash(), ... |
| Validation | ValidateUtil, ... | isValid(), check(), ... |
| Logging | LogContext, ... | push(), ... |
| Exception | VnpayInvalidException, ErrorCode, ... | throw patterns |
| ... | | |

---

## Step 4 — Write `knowledge_common_utils.md`

> ⚠️ ALL class names, methods MUST be REAL values — no placeholders.

### Output template:

````markdown
# Common Utils & Shared Services Knowledge

_Generated from codebase analysis — YYYY-MM-DD._

---

## Quick Lookup Table

| Dùng để làm gì (Purpose) | Khi nào dùng (When to use Class) | Dùng như thế nào (How to use Method) |
|---|---|---|
| Format ngày tháng (Date) | `vn.vnpay...util.datetime.TimeUtil` | `toStringDate(LocalDateTime)` |
| Làm tròn số tiền (Amount) | `vn.vnpay...util.CurrencyUtil` | `roundAmount(BigDecimal, Currency)` |
| Kiểm tra chuỗi rỗng | `org.apache.commons.lang3.StringUtils` | `StringUtils.isBlank(str)` |
| Ném lỗi nghiệp vụ chung | `VnpayInvalidException` | `new VnpayInvalidException(ErrorCode.XXX)` |
| Ghi Log hệ thống | `LogContext` | `LogContext.push(LogType.TRACING, data)` |
| {more...} | | |

---

## DateTime Utilities
### {ClassName}
- Package: `{full.package}`
- Type: Static utility
- Methods:
  | Method | Signature | Description |
  |---|---|---|
  | {name} | {params → return} | {desc} |

---

## String Utilities
{Same format}

---

## Currency / Number Utilities
{Same format}

---

## JSON / Serialization Utilities
{Same format}

---

## Security / Crypto Utilities
{Same format}

---

## Validation Utilities
{Same format}

---

## Logging Infrastructure
### LogContext
{How to use, LogType enum, push patterns}

---

## Exception Infrastructure
### VnpayInvalidException
{Constructor, ErrorCode enum/interface, usage pattern}

### Error Code Registry
{Common error codes: SUCCESS, INVALID, UNSUPPORTED, ...}

---

## Shared Services (Injectable)
| Service | Interface | Package | Key Methods |
|---|---|---|---|

---

## Constants & Enums Registry
| Class | Package | Purpose | Key Values |
|---|---|---|---|

---

## Anti-Duplication Rules
{Numbered list: before writing new code, MUST check these utilities first}

1. Before writing a Date formatter → check `TimeUtil`
2. Before writing an amount rounder → check `CurrencyUtil`
3. Before throwing an exception → check `ErrorCode` enum
4. Before writing JSON serialization → check `JsonUtil`
5. ... (real list from source)
````

---

## Step 5 — Self-Verification

1. [ ] ALL util classes documented with methods
2. [ ] Quick Lookup Table present and populated
3. [ ] ALL categories covered (DateTime, String, Currency, JSON, Security, Validation, Logging, Exception)
4. [ ] Shared Services listed with injection pattern
5. [ ] Constants/Enums registry present
6. [ ] Anti-Duplication Rules list derived from actual utils
7. [ ] NO `{placeholder}` remains
8. [ ] Method signatures from actual source

---

## Guardrails

- DO NOT assume util class names — scan first
- DO NOT skip reading methods — list actual public methods
- Focus on `common/` module but DO NOT ignore reusable utils at service-level
- Exclude test utils, build scripts, generated code
- Quick Lookup Table is the MOST IMPORTANT section — MUST fully map "Need → Use"
- If a util has overloaded methods, list the main variant and note "has N overloads"
