---
name: learn-architecture
description: Learn project directory structure and standard request flow through all layers — scan, trace, and document the full lifecycle of a request from Controller to DB into a structured knowledge file for LLM reuse.
---

## Purpose

Automatically scan the project source code, extract and document the **Project Architecture DNA** so the LLM can:
1. Understand which file types belong where (directory convention)
2. Trace the standard request flow through all layers
3. Create new modules/endpoints following the correct structure
4. Know required base classes, annotations, and patterns per layer

**Output filename**: `knowledge_architecture.md`
**Output directory**: Determined by caller. Default: `base_knowledge/structures/propose/`.

---

## Input

- **Project root** (required): project root directory
- **Scope** (optional): specific service name, defaults to scan ALL

---

## Step 1 — Find Entry Points

Do NOT assume structure. **Scan the actual codebase**.

### 1a. Scan Project Root — Find all services

```
list_dir({project_root})
```

**Determine**:
- Which directories are microservices? (typically contain `build.gradle` or `pom.xml` + `src/`)
- Which directories are shared libraries? (common/, libs/)
- Is there a config-service, gateway, or client module?
- Does each service have an `adapter/` (cross-service communication)?

```
find_by_name: Pattern="build.gradle", SearchDirectory={project_root}
find_by_name: Pattern="pom.xml", SearchDirectory={project_root}
```

### 1b. Scan internal structure of each service

```
list_dir({service}/src/main/java/...)
```

**Determine**:
- Layout: flat package or module-based? (`module/{feature}/controller/handler/factory/...`)
- Are controllers split by channel? (app/, web/, admin/)
- Are handlers split by CQRS? (command vs query)
- Naming convention: what prefix/suffix per layer?

### 1c. Find Controllers — Layer 1

```
grep_search: "@RestController" in {service_path}
grep_search: "@Controller" in {service_path}
grep_search: "extends Base.*Controller" in {service_path}
```

**Record**:
- Base controller class: name, package, key methods (e.g., `execute()`)
- Interface + impl split? (`I{Name}Controller` + `App{Name}Controller`)
- Required annotations: `@RestController`, `@RequestMapping`, `@Tag`, `@LoggingInsert`, ...
- Pattern: does the controller contain business logic or only delegate?

### 1d. Find Business Logic Layer — Layer 2

```
grep_search: "extends Base.*Handler" in {service_path}
grep_search: "@Component" + "Handler" in {service_path}
grep_search: "extends.*UseCase" in {service_path}
grep_search: "extends.*Service" in {service_path}
```

**Determine**: which pattern does the project use?
- **CQRS Handler**: `BaseHandler<Request, Response>` + MessageBus dispatch?
- **UseCase pattern**: `Base*UseCase` classes?
- **Service layer**: `@Service` classes?
- **Or a combination?**

**Record**:
- Base class name and location
- Routing mechanism: annotation-based? MessageBus? Direct injection?
- Abstract methods that must be implemented
- Optional methods that can be overridden

### 1e. Find Data Access Layer — Layer 3

```
grep_search: "extends Base.*Factory" in {service_path}
grep_search: "extends.*Repository" in {service_path}
grep_search: "@Repository" in {service_path}
grep_search: "extends Base.*DataFactory" in {service_path}
```

**Determine**: which pattern does the project use?
- **Factory pattern**: `BaseCrudDataFactory`, `BaseClientDataFactory`?
- **Direct Repository**: `JpaRepository`, `MongoRepository`?
- **DAO pattern?**
- **Active Record?**

**Record**:
- Base class + interface hierarchy
- Cache mechanism (Redis? Local? None?)
- Entity → Model conversion pattern

### 1f. Find Request/Response Models

```
grep_search: "extends Base.*Request" in {service_path}
grep_search: "extends Base.*Response" in {service_path}
find_by_name: Pattern="*Request.java" in {service_path}
find_by_name: Pattern="*Response.java" in {service_path}
```

**Record**:
- Base request/response classes
- DTO pattern: request ≠ response? Intermediate model?
- Validation annotations: `@NotNull`, `@NotBlank`, `@Valid`?

### 1g. Find Cross-Service Communication

```
grep_search: "@FeignClient" in {project_root}
grep_search: "proto" + "service" in {project_root}  # gRPC
grep_search: "extends Base.*Client.*Factory" in {project_root}
find_by_name: Extensions=["proto"], SearchDirectory={project_root}
```

**Determine**:
- Feign clients? gRPC? REST template? WebClient?
- Adapter module pattern?
- Where are shared interfaces/models?

### 1h. Find Adapter Structure (if present)

```
find_by_name: Pattern="adapter", Type="directory", SearchDirectory={project_root}
```

**Record**:
- Feign client interfaces
- Shared factory interfaces
- Auto-configuration classes
- Protobuf definitions

---

## Step 2 — Trace Request Flow

Choose **at least 3 representative flows** (simplest, most complex, special) and trace end-to-end:

### For each flow:

1. **HTTP endpoint**: method + URL + controller class
2. **Controller logic**: delegate or handle directly?
3. **Routing mechanism**: MessageBus dispatch? Direct call? AOP?
4. **Business logic**: Handler/UseCase/Service class + base class
5. **Data access**: Factory/Repository + cache layer
6. **External calls** (if any): Feign/gRPC/REST
7. **Response building**: factory/builder pattern

### Format:
```
Flow: {Description}
1. Client → POST /{path}
2. {Controller}.{method}(Request)
   → calls {mechanism}
3. {Handler/UseCase}.handle(Request)
   → calls {Factory/Service}
4. {Factory/Repository}.{method}()
   → {data source}
5. Response → back through layers
```

---

## Step 3 — Write `knowledge_architecture.md`

> ⚠️ ALL class names, packages MUST be REAL values from source — no placeholders.

### Output template:

````markdown
# Project Architecture Knowledge

_Generated from codebase analysis — YYYY-MM-DD._
_Scoped to: {scope}._

---

## Project Overview

### Services
{Table: service name | adapter (y/n) | module count | description}

### Common Libraries
{Table: library name | package | purpose}

---

## Directory Convention

### Service-Level Structure
{Actual tree diagram}

### Module-Level Structure (Standard Pattern)
{Actual tree diagram — naming convention per directory}

### Adapter Structure (if present)
{Actual tree diagram}

---

## Request Flow — Standard Pattern

### Architecture Diagram
{ASCII diagram: Client → Controller → [Routing] → Handler → Factory → DB/API}

### Chi tiết các Tầng (Layer Details)

#### Layer 1 — Controller
- **Dùng để làm gì (Purpose):** {Tiếp nhận Request HTTP, Mapping URL, Route tới tầng Business}
- **Khi nào dùng (When to use):** {Mọi Request từ Client (App/Web) gọi vào Service đều phải qua Controller. Có chia Interface tách biệt không?}
- **Dùng như thế nào (How to use):** 
  - Annotations bắt buộc: {@RestController, @RequestMapping...}
  - Code Template chuẩn: {REAL code snippet}

#### Layer 2 — Business Logic (Handler/UseCase/Service)
- **Dùng để làm gì (Purpose):** {Xử lý nghiệp vụ lõi (validate, rule, compose data)}
- **Khi nào dùng (When to use):** {Chứa 100% logic nghiệp vụ. Tách rời khỏi Framework HTTP}
- **Dùng như thế nào (How to use):**
  - Cơ chế nhận Request: {CQRS MessageBus hay Inject trực tiếp?}
  - Cách override base: {Các hàm bắt buộc ghi đè}
  - Code Template chuẩn: {REAL code snippet}

#### Layer 3 — Data Access (Factory/Repository)
- **Dùng để làm gì (Purpose):** {Giao tiếp Database, Cache, Convert Entity ↔ Model}
- **Khi nào dùng (When to use):** {Bất cứ khi nào cần read/write state từ Storage}
- **Dùng như thế nào (How to use):**
  - Cơ chế Cache: {Có dùng Redis không? Inject CacheManager?}
  - Chi tiết xem tại: `knowledge_factory.md` và `knowledge_dto_entity.md`

#### Layer 4 — External / 3rd Party
- **Dùng để làm gì (Purpose):** {Giao tiếp service khác hoặc hệ thống bên ngoài (Bank GW)}
- **Dùng như thế nào (How to use):** {Thực hiện qua FeignClient hay Gateway Adapter?}
  - Chi tiết xem tại: `knowledge_thirdparty_call.md`

---

## Naming Conventions
{Table: Item | Convention | Example — ALL from real source}

## Annotations & Imports per Layer
{Per layer: required annotations, base class, imports}

---

## Key Patterns

### Controller Pattern
{Interface + Impl? Channel split (App/Web)? Delegate only?}

### Business Logic Pattern
{CQRS? UseCase? Service? Routing mechanism}

### Data Access Pattern
{Factory? Direct Repository? Cache layer?}

### Cross-Service Communication
{Feign? gRPC? Adapter pattern?}

---

## Traced Request Flows (Examples)

### Example 1: {Simple CRUD flow}
### Example 2: {Complex multi-layer flow}
### Example 3: {Cross-service or external API flow}

---

## Cross-References
{Links to related knowledge files}
````

---

## Step 4 — Self-Verification

1. [ ] ALL services listed with correct module counts
2. [ ] Directory tree matches REAL structure
3. [ ] Request flow diagram accurate end-to-end
4. [ ] ALL naming conventions from real examples
5. [ ] At least 3 traced flows included
6. [ ] NO `{placeholder}` remains
7. [ ] Code snippets from actual source
8. [ ] Base class table has real package paths
9. [ ] Cross-references present

---

## Guardrails

- DO NOT invent directory/class names — only document what FOUND
- DO NOT assume directory structure — scan first
- DO NOT hardcode pattern names (CQRS, UseCase, MVC) — discover from source
- Code snippets MUST be actual source
- Output MUST reflect REAL project as-is
- If pattern missing in a service, note it
