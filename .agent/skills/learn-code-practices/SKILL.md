---
name: learn-code-practices
description: Learn runtime coding practices — logging, exception handling, validation, response building, and other common operations used during code generation. Document patterns as a knowledge file for LLM reuse.
---

## Purpose

Automatically scan the source code, extract and document **Runtime Code Practice DNA** so the LLM can:
1. Apply correct logging patterns at every layer (Controller, Handler, Factory, Gateway)
2. Throw and handle exceptions following project standards
3. Build API responses using the correct factory/wrapper
4. Validate inputs using the right annotations and utility methods
5. Follow established patterns for common operations (null check, type conversion, MDC tracing, etc.)

**Output filename**: `knowledge_code_practices.md`
**Output directory**: Determined by caller. Default: `base_knowledge/structures/apply/`.

---

## Input

- **Project root** (required): project root directory
- **Scope** (optional): specific service, defaults to scan ALL

---

## Step 1 — Logging Patterns

### 1a. Find Logging Infrastructure Classes

```
grep_search: "class LogContext" in {project_root}
grep_search: "enum LogType" in {project_root}
grep_search: "LogContext" in {project_root}/common
find_by_name: Pattern="LogContext.java", SearchDirectory={project_root}
find_by_name: Pattern="LogType.java", SearchDirectory={project_root}
find_by_name: Pattern="*Log.java", SearchDirectory={project_root}/common
```

**Read and record**:
- `LogContext` class: all public methods (`push()`, `get()`, `clear()`, etc.)
- `LogType` enum: all values (TRACING, REQUEST, RESPONSE, EXCEPTION, CALL_REST, etc.)
- Push method signatures: `push(LogType, Object)`, `push(LogType, String, Object...)`

### 1b. Find Controller-Level Logging

```
grep_search: "@LoggingInsert" in {project_root}
find_by_name: Pattern="LoggingInsert.java", SearchDirectory={project_root}
```

**Record**:
- What `@LoggingInsert` does (AOP annotation? Interceptor?)
- Where it is placed (controller class? method? both?)
- What it logs automatically (request body? response? timing?)

### 1c. Find Handler-Level Logging

```
grep_search: "LogContext.push(LogType.TRACING" in {project_root}
grep_search: "LogContext.push(LogType.REQUEST" in {project_root}
grep_search: "LogContext.push(LogType.RESPONSE" in {project_root}
grep_search: "LogContext.push(LogType.EXCEPTION" in {project_root}
```

Select **5-10 representative handlers** and read their logging patterns:
- When is TRACING used? (business decisions, flow branching, data traces)
- When is REQUEST/RESPONSE used? (external API calls, cross-service calls)
- When is EXCEPTION used? (catch blocks, error conditions)
- Format: string message? Object payload? Formatted string with `{}`?

### 1d. Find Factory/Service-Level Logging

```
grep_search: "LogContext.push" in {project_root}/common
```

**Record**:
- Does base factory/handler auto-log? Or must subclass log explicitly?
- Cache miss/hit logging?
- DB query logging?

### 1e. Find MDC / Trace ID Pattern

```
grep_search: "MDC" in {project_root}/common
grep_search: "requestId\|traceId\|correlationId" in {project_root}/common
```

**Record**:
- MDC fields set automatically: requestId, sessionId, customerId, etc.
- Where are they set? (Filter? Interceptor? Base handler?)
- How to access: `MDC.get("requestId")`

### 1f. Find SLF4J / Lombok @Slf4j Usage

```
grep_search: "@Slf4j" in {project_root}
grep_search: "log.info\|log.error\|log.warn\|log.debug" in {project_root}
```

**Record**:
- Is `@Slf4j` used alongside `LogContext`?
- When to use `log.*` vs `LogContext.push()`?
- Convention: `log.error` for system errors, `LogContext.push` for business tracing?

---

## Step 2 — Exception Handling Patterns

### 2a. Find Exception Classes

```
find_by_name: Pattern="*Exception.java", SearchDirectory={project_root}
find_by_name: Pattern="*Error.java", SearchDirectory={project_root}
```

**For EACH exception class, read and record**:
- Class name + package
- Extends which base? (RuntimeException? VnpayException? custom?)
- Constructor params (ErrorCode? message? data?)
- When to throw

### 2b. Find ErrorCode Interface/Enum

```
grep_search: "interface.*ErrorCode\|enum.*ErrorCode\|IErrorCode" in {project_root}
grep_search: "implements.*ErrorCode" in {project_root}
find_by_name: Pattern="*ErrorCode*.java", SearchDirectory={project_root}
```

**Record**:
- Base interface: `IErrorCode`? `ErrorCode`?
- Methods: `getCode()`, `getMessage()`, `getHttpStatus()`
- Per-service error code enums: naming pattern, code format
- Global vs service-specific error codes

### 2c. Find Global Exception Handler

```
grep_search: "extends VnpayGlobalExceptionHandler" in {project_root}
grep_search: "@RestControllerAdvice" in {project_root}
grep_search: "@ExceptionHandler" in {project_root}
```

**Read the base `VnpayGlobalExceptionHandler` and record**:
- Handled exception types: VnpayInvalidException, VnpayClientConnectionTimeoutException, MethodArgumentNotValidException, etc.
- Response format per exception type
- How error response is built (ResponseFactory?)
- HTTP status mapping logic

**Read 2-3 service-specific exception handlers** and record:
- Custom `@ExceptionHandler` methods
- How they override/extend the global handler

### 2d. Find Exception Throwing Patterns

```
grep_search: "throw new VnpayInvalidException" in {project_root}
grep_search: "throw new VnpayClient" in {project_root}
```

Select **10+ examples** and classify patterns:
- Throw with ErrorCode enum: `throw new VnpayInvalidException(ErrorCode.XXX)`
- Throw with message: `throw new VnpayInvalidException(ErrorCode.XXX, "message")`
- Throw with data: `throw new VnpayInvalidException(ErrorCode.XXX, data)`
- Conditional throw: `if (!valid) throw ...`
- Guard clause pattern: check-and-throw at method start

---

## Step 3 — Validation Patterns

### 3a. Find Bean Validation Annotations

```
grep_search: "@NotNull\|@NotBlank\|@NotEmpty\|@Size\|@Min\|@Max" in {project_root}
grep_search: "@Valid" in {project_root}
grep_search: "@Validated" in {project_root}
```

**Record**:
- Where are validations placed? (Request DTO fields? Method params?)
- Common combinations: `@NotNull @NotBlank`, `@Size(min, max)`
- Controller `@Valid` usage: `@RequestBody @Valid Request request`

### 3b. Find Custom Validators

```
grep_search: "implements Validator\|implements ConstraintValidator" in {project_root}
grep_search: "@interface.*Valid\|@interface.*Check" in {project_root}
find_by_name: Pattern="*Validator.java", SearchDirectory={project_root}
```

### 3c. Find Programmatic Validation (in Handlers)

```
grep_search: "ValidateUtil\|ValidationUtil\|Preconditions" in {project_root}
grep_search: "Objects.requireNonNull\|StringUtils.isEmpty\|CollectionUtils.isEmpty" in {project_root}
```

**Record patterns**:
- Guard clause at top of `preHandle()`: validate + throw
- Util-based: `ValidateUtil.check(condition, errorCode)`
- Apache/Spring util: `StringUtils.isBlank()`, `CollectionUtils.isEmpty()`
- Null safety: `Optional.ofNullable()`, `Objects.nonNull()`

---

## Step 4 — Response Building Patterns

### 4a. Find ResponseFactory / Response Wrapper

```
grep_search: "class ResponseFactory\|ResponseFactory" in {project_root}/common
grep_search: "BaseResponse\|ApiResponse\|ResponseWrapper" in {project_root}/common
grep_search: "ResponseEntity" in {project_root}
```

**Read and record**:
- Response wrapper class: structure (code, message, data, success)
- ResponseFactory methods: `success(data)`, `error(errorCode)`, etc.
- Controller return type: `ResponseEntity<BaseResponse<T>>`?
- Handler return: raw response object (wrapped by base handler)?

### 4b. Find Response Building in Handlers

Select **5-10 handlers** and read how they build responses:
- Builder pattern: `Response.builder().xxx(value).build()`?
- Direct set: `response.setXxx(value)`?
- Factory conversion: `model → response` mapping?

---

## Step 5 — Common Operations During Code Generation

### 5a. Null Safety Patterns

```
grep_search: "Optional.ofNullable\|Optional.of\|Optional.empty" in {project_root}
grep_search: "ObjectUtils\|Objects.nonNull\|Objects.isNull" in {project_root}
```

### 5b. Type Conversion / Mapping

```
grep_search: "MapperUtil\|ModelMapper\|ObjectMapper" in {project_root}
grep_search: "convertValue\|readValue\|writeValueAsString" in {project_root}
```

### 5c. Date/Time Operations

```
grep_search: "LocalDateTime\|LocalDate\|ZonedDateTime\|Instant" in {project_root}/common
grep_search: "TimeUtil\|DateUtil\|DateTimeFormatter" in {project_root}
```

### 5d. String Formatting / Template

```
grep_search: "String.format\|MessageFormat\|TemplateUtil" in {project_root}
```

### 5e. Collection Operations

```
grep_search: "stream().map\|stream().filter\|Collectors.toList\|Collectors.toMap" in {project_root}
```

Select 5-10 examples of stream usage patterns.

### 5f. Async / Event Patterns

```
grep_search: "@Async\|CompletableFuture\|EventPublisher" in {project_root}
grep_search: "KafkaTemplate\|KafkaProducer\|@KafkaListener" in {project_root}
```

---

## Step 6 — Write `knowledge_code_practices.md`

> ⚠️ ALL class names, methods, annotations MUST be REAL values from source — no placeholders.

### Output template:

````markdown
# Code Practices Knowledge

_Generated from codebase analysis — YYYY-MM-DD._

---

## Quick Reference

## Quick Reference

| Dùng để làm gì (Purpose) | Khi nào dùng (When to use) | Dùng như thế nào (How to use: Pattern & Example) |
|---|---|---|
| Ghi chú vết (Trace) logic nghiệp vụ | Các nhánh quan trọng trong code | `LogContext.push(LogType.TRACING, "Account validated")` |
| Log Request/Response | Trước/sau khi gọi API ngoài | `LogContext.push(LogType.REQUEST, apiRequest)` |
| Báo lỗi nghiệp vụ | Xảy ra lỗi business logic | `throw new VnpayInvalidException(ErrorCode.INVALID_ACCOUNT)` |
| Validate input required | DTO nhận từ client | `@NotNull private String accountNo;` |
| Chặn lỗi ở đầu hàm (Guard clause) | Đầu hàm `preHandle` | `if (x == null) throw new VnpayInvalidException(...)` |
| Xây dựng Response | Cuối hàm `aroundHandle` | `Response.builder().xxx(value).build()` |
| {more...} | | |

---

## 1. Logging

### 1.1 Logging Infrastructure
{LogContext class, LogType enum with ALL values, push method signatures}

### 1.2 Controller Logging
{@LoggingInsert annotation — what it does, where to place it}

### 1.3 Handler Logging Rules
| LogType | When to Use | Example |
|---|---|---|
| TRACING | Business flow decisions, data validation results | `LogContext.push(LogType.TRACING, "Validated OK")` |
| REQUEST | Before external/cross-service calls | `LogContext.push(LogType.REQUEST, requestObj)` |
| RESPONSE | After receiving external response | `LogContext.push(LogType.RESPONSE, responseObj)` |
| EXCEPTION | In catch blocks, error conditions | `LogContext.push(LogType.EXCEPTION, exMsg)` |
| CALL_REST | REST API call details | `LogContext.push(LogType.CALL_REST, clientLog)` |

### 1.4 Logging Best Practices
{MUST-follow rules from actual code patterns}

### 1.5 MDC / Trace ID
{Auto-set fields, where set, how to access}

### 1.6 SLF4J vs LogContext Decision
{When to use which}

---

## 2. Exception Handling

### 2.1 Exception Class Hierarchy
{Diagram: RuntimeException → VnpayException → VnpayInvalidException / VnpayClientConnectionTimeoutException / ...}

### 2.2 ErrorCode Interface & Per-Service Enums
{Base interface, methods, per-service enum pattern, code format}

### 2.3 How to Throw Exceptions
| Pattern | Usage | Example |
|---|---|---|
| Simple ErrorCode | Most common | `throw new VnpayInvalidException(ErrorCode.INVALID)` |
| With message | Extra context | `throw new VnpayInvalidException(ErrorCode.INVALID, "account not found")` |
| With data | Return data in error | `throw new VnpayInvalidException(ErrorCode.INVALID, dataObj)` |

### 2.4 Guard Clause Pattern
{Code snippet showing preHandle validation + throw pattern}

### 2.5 Global Exception Handler
{VnpayGlobalExceptionHandler base, per-service extends, response format}

### 2.6 Exception Handling Rules
{Numbered MUST-follow rules}

---

## 3. Validation

### 3.1 Bean Validation on DTOs
{Common annotations: @NotNull, @NotBlank, @Size, @Valid}

### 3.2 Programmatic Validation in Handlers
{Guard clause patterns, util methods}

### 3.3 Custom Validators (if present)
{Custom annotation validators}

### 3.4 Validation Rules
{Where to validate: DTO annotations vs handler preHandle vs factory}

---

## 4. Response Building

### 4.1 Response Wrapper Structure
{BaseResponse class, fields: code, message, data, success}

### 4.2 ResponseFactory
{Methods, usage in controller vs handler}

### 4.3 Handler Response Pattern
{Builder pattern examples from real handlers}

### 4.4 Response Rules
{When to use ResponseEntity vs raw return}

---

## 5. Common Operations

### 5.1 Null Safety
{Patterns: Optional, Objects.nonNull, guard clauses}

### 5.2 Type Conversion / Mapping
{MapperUtil, ObjectMapper patterns}

### 5.3 Date/Time Operations
{TimeUtil, standard patterns}

### 5.4 String Operations
{TemplateUtil, String.format}

### 5.5 Collection / Stream Patterns
{Common stream patterns from source}

### 5.6 Async / Event Patterns
{@Async, Kafka, event publisher patterns}

---

## 6. Code Generation Checklist

When generating new code, ALWAYS follow:

1. [ ] Controller: add `@LoggingInsert`
2. [ ] Handler preHandle: add guard clauses with `LogContext.push(LogType.TRACING, ...)`
3. [ ] Handler aroundHandle: add `LogContext.push(LogType.TRACING, ...)` for key decisions
4. [ ] External API calls: add `LogContext.push(LogType.REQUEST/RESPONSE, ...)`
5. [ ] Exception: throw `VnpayInvalidException(ErrorCode.XXX)` with correct ErrorCode
6. [ ] Validation: use `@NotNull`/`@NotBlank` on DTO, guard clauses in handler
7. [ ] Response: use Builder pattern matching existing handler responses
8. [ ] {more from real patterns...}

---

## 7. Anti-Patterns (DO NOT)

1. DO NOT use `System.out.println()` — use LogContext
2. DO NOT catch and swallow exceptions silently
3. DO NOT hardcode error messages — use ErrorCode enum
4. DO NOT skip logging in catch blocks
5. {more from real patterns...}

---

## Cross-References
{knowledge_handler.md, knowledge_common_utils.md, knowledge_architecture.md}
````

---

## Step 7 — Self-Verification

1. [ ] ALL LogType enum values documented
2. [ ] @LoggingInsert usage documented
3. [ ] Exception hierarchy documented with real class names
4. [ ] ErrorCode pattern documented per-service
5. [ ] Guard clause pattern with real examples
6. [ ] Response building pattern documented
7. [ ] Validation patterns documented (annotation + programmatic)
8. [ ] Code Generation Checklist present
9. [ ] Anti-Patterns list present
10. [ ] NO `{placeholder}` remains

---

## Guardrails

- DO NOT assume logging framework — scan for actual LogContext/SLF4J usage
- DO NOT assume exception class names — find actual from source
- DO NOT skip reading handler logging patterns — they define the standard
- Code snippets MUST be from actual source (shortened OK, invented NOT OK)
- Cross-reference `knowledge_common_utils.md` for utility methods
- Cross-reference `knowledge_handler.md` for handler lifecycle hooks
- Quick Reference table is the MOST IMPORTANT section — developers use it daily
