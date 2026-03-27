---
name: learn-factory
description: Learn factory patterns from existing source code — scan, classify, and document all Factory types plus cache invalidation mechanisms into a structured knowledge file for LLM reuse.
---

## Purpose

Automatically scan the source code, extract and document the **Factory Pattern DNA** so the LLM can:
1. Understand how many factory types exist (CRUD, Client/API, ETag, Aggregator, ...)
2. Know which base class to use for each type, constructor pattern, and override methods
3. Create new factories following the correct structure, cache, and naming conventions
4. Understand cache invalidation mechanisms (pub/sub, ETag, manual)

**Output filename**: `knowledge_factory.md`
**Output directory**: Determined by caller. Default: `base_knowledge/structures/propose/`.

---

## Input

- **Project root** (required): project root directory
- **Scope** (optional): specific service/module, defaults to scan ALL

---

## Step 1 — Find Entry Points

Do NOT assume base class names or patterns. **Scan the actual codebase**.

### 1a. Find all Factory Base Classes

```
grep_search: "abstract class Base.*Factory" in {project_root}
grep_search: "abstract class Base.*DataFactory" in {project_root}
grep_search: "class.*Factory.*<" in {project_root}  # generic factory classes
```

**Determine**:
- How many factory types exist? (CRUD? Client? ETag? Aggregator? Custom?)
- Real name of each base class (may NOT be "CrudDataFactory"/"ClientDataFactory")
- Package location
- Generic type params: order and meaning

### 1b. Find all Factory Implementations (per type)

For EACH base class found in 1a:

```
grep_search: "extends {BaseFactoryClass}" in {project_root}
```

For EACH implementation, read the file and record:
- Class name + package + service/module
- Interface it implements
- Generic types (exact, correct order)
- Constructor: injection pattern (ObjectProvider? Direct? @Qualifier?)
  - **CRITICAL**: `BaseCrudDataFactory` constructor takes `(IVnpayCacheManager, Repository)` — 2 params
  - **CRITICAL**: `BaseClientDataFactory` constructor takes `(IVnpayCacheManager)` ONLY — 1 param, NO Repository
  - Record exact `@Qualifier` value if present (e.g., `"commonSecurityCacheManager"`)
- Bean name: `@Component("XXX")` — the XXX value
- Overridden methods: list all, especially:
  - **For BaseCrudDataFactory**: `convertToModel(Entity)`, `getCacheClass()`, `listQuery(Filter)`, `cacheFactory()`
  - **For BaseClientDataFactory**: `callClient(Filter)`, `convertToModel(AgwResponse, Filter)`, `makeKey(Filter)`, `getCacheClass()`, `cacheFactory()`
  - Convert methods: entity↔model
  - Cache config: TTL, async, enable/disable
  - Query methods: custom DB queries
  - Cache key: custom key strategy
- **What client does it inject?** Record `IAgwXxxClient` interface name (NOT Feign — see learn-thirdparty-call)
- Special: does it also implement ETag/Reload interfaces?

### 1c. Find Factory Interfaces

```
grep_search: "interface I.*Factory" in {project_root}
grep_search: "extends IDataFactory" in {project_root}
grep_search: "extends IClientDataFactory" in {project_root}
grep_search: "extends IETag.*Factory" in {project_root}
```

**Record**:
- Interface hierarchy: base interface → domain interface
- Generic type params
- Custom business methods (beyond standard CRUD)

> ⚠️ **CRITICAL**: Tìm hiểu base interface (`IClientDataFactory`, `IDataFactory`, `ICrudDataFactory`) cung cấp **method gì** cho handler gọi. Ví dụ:
> - `IClientDataFactory` cung cấp `getData(Filter)`, `getCacheModel(Filter)`
> - `IDataFactory` cung cấp `getModelById(ID)`, `list(Filter)`
> 
> Domain interface (ví dụ `ISavingDetailFactory`) **PHẢI extends** base interface, body thường **rỗng** (KHÔNG tạo custom method).
> Handler gọi factory qua method do base interface cung cấp (`getData(filter)`), KHÔNG gọi custom method.

```
# Scan base interface internals:
grep_search: "interface IClientDataFactory" in {project_root}  # may be in JAR → check imports
grep_search: "interface IDataFactory" in {project_root}
# Check what methods handler actually calls:
grep_search: "\.getData(" in {project_root}  # IClientDataFactory method
grep_search: "\.getCacheModel(" in {project_root}  # IClientDataFactory method
grep_search: "\.getModelById(" in {project_root}  # IDataFactory method
grep_search: "\.list(" in factory-related handler files
```

**MUST answer**:
1. Base interface nào tồn tại? (`IClientDataFactory`, `IDataFactory`, `ICrudDataFactory`, ...)
2. Mỗi base interface cung cấp method gì? (tên, signature, return type)
3. Domain interface có extends base interface không? (must YES)
4. Domain interface có body rỗng hay có custom method?
5. Handler gọi factory qua method nào? (phải khớp với base interface method)

### 1d-extra. Find Module-Level Base Filter Classes

Factories using `BaseClientDataFactory` often have a shared module-level filter:

```
grep_search: "implements IFilter" in {project_root}
grep_search: "extends.*Filter" in {project_root}
grep_search: "fromBaseSessionRequest\|fromRequest" in {project_root}
```

**Look for**:
- Base filter class per module (e.g., `SavingFilter`, `TransferFilter`) implementing `IFilter`
- Builder helper: `fromBaseSessionRequest(BaseSessionRequest)` — auto-extracts customerId, sessionId, cifCore
- Feature-specific filters extending the base filter
- Annotations: `@SuperBuilder(toBuilder = true)`, `@Getter`, `AccessLevel.PROTECTED`

**WHY**: If not documented, LLM will create plain POJO filters missing the builder helper pattern.

### 1d. Find Cache Mechanism

```
grep_search: "IVnpayCacheManager\|CacheManager\|@Cacheable\|CacheConfigFactory" in {project_root}
grep_search: "singleTtl\|collectionTtl\|cacheModel\|cacheCollection\|cacheAsync" in {project_root}
grep_search: "cachePutModel\|cacheEvict\|makeKey\|makeCollectionKey" in {project_root}
```

**Determine**:
- Cache provider: Redis? Local? Caffeine? Spring Cache?
- Cache config pattern: factory method? Annotation? Properties?
- TTL patterns: Duration-based? Integer seconds?
- Cache qualifiers: per-domain? Security vs non-security?

### 1e. Find Cache Invalidation / Reload Mechanism

```
grep_search: "ReloadCache\|CacheSubscriber\|CacheListener\|CacheInvalidat" in {project_root}
grep_search: "extends.*ReloadCache.*Subscriber" in {project_root}
grep_search: "pub.*sub\|subscribe\|onMessage\|getTopics" in {project_root}
```

**Determine**:
- Uses pub/sub (Redis)? Event listener? Manual?
- Bean name ↔ factory mapping rule
- Topic/channel name
- Guard logic (skip sensitive factories)
- ETag eviction logic

### 1f. Find ETag / Conditional Fetch Pattern (if present)

```
grep_search: "ETag\|etag\|IETag" in {project_root}
grep_search: "304\|NotModified\|conditional" in {project_root}
grep_search: "BaseGetDataByETag\|ETagHandler" in {project_root}
```

**Determine**:
- Does an ETag pattern exist?
- ETag factory: base class, interface
- ETag type enum: values
- Handler pattern for ETag endpoints
- 2 variants: single entity + ETag? Aggregator + ETag?

### 1g. Find Constants / Bean Naming

```
grep_search: "ReloadCacheFactoryConstants\|FACTORY_CONSTANT\|BEAN_NAME" in {project_root}
find_by_name: Pattern="*Constants*", Extensions=["java"], SearchDirectory={project_root}
```

**Record**:
- Constant file(s): location, values
- Rule: bean name = entity table name? Or a different rule?
- Current values: to know where to add constants for new factories

### 1h. Find Handler ↔ Factory Call Pattern

> ⚠️ **CRITICAL STEP** — nếu bỏ qua, LLM sẽ tạo custom method trên interface và gọi sai trong handler.

Chọn 2-3 handler đang inject factory (đã tìm thấy ở Step 1b), đọc handler code để xác định:

```
# Tìm handler inject factory interface:
grep_search: "private final I.*Factory" in {project_root}/*/handler/
# Đọc handler code → xem gọi factory method nào:
grep_search: "Factory\.get" in {project_root}/*/handler/
```

**MUST record**:
1. Handler inject factory qua interface nào? (ví dụ: `ISavingDetailFactory`)
2. Handler gọi method gì trên factory? (ví dụ: `.getData(filter)`, `.getCacheModel(filter)`, `.getModelById(id)`)
3. Method đó do base interface hay custom interface cung cấp?
4. Filter được build như thế nào trong handler? (builder chain, `fromBaseSessionRequest(request)` ở cuối)

**WHY**: Nếu không document, LLM sẽ:
- Tạo plain interface với custom method (SAI)
- Gọi custom method trong handler (SAI)
- Thay vì dùng base interface method `getData(filter)` (ĐÚNG)

---

## Step 2 — Analyze Base Class Internals

For EACH base factory class found in Step 1a, read the source and document:

### Per base class:

- **Constructor signature**: params + annotations
  - **DISTINGUISH**: `BaseCrudDataFactory(IVnpayCacheManager, Repository)` vs `BaseClientDataFactory(IVnpayCacheManager)` — different param counts!
- **Generic type order**: e.g., `<ID, Model, EntityID, Entity, Repository>` — EXACT
- **Abstract methods** subclass MUST implement:
  - Method name, signature, return type, purpose
  - Can return null? Throw exception?
  - **FOR BaseClientDataFactory**: `callClient(Filter)` is the MOST IMPORTANT method — it calls the AGW client
  - **FOR BaseClientDataFactory**: `convertToModel(AgwResponse, Filter)` — note signature takes 2 params (response + filter), NOT just 1
- **Optional override methods**:
  - Method name, default behavior, when to override
- **Cache lifecycle**:
  - Read: check cache → miss → load → cache → return
  - Write: create/update → save DB → update cache
  - Invalidation: manual? Subscriber? Auto?
- **Key patterns**:
  - Single item key: by ID
  - Collection key: by filter? Custom `makeCollectionKey()`?

---

## Step 3 — Classify and Count

### 3a. Classification Table

| Factory Type | Count | Base Class | Interface | Data Source | Cache Mechanism |
|---|---|---|---|---|---|
| {Type 1} | N | {class} | {interface} | {DB/API/Composed} | {Redis/Local/None} |
| {Type 2} | N | {class} | {interface} | {source} | {cache} |
| ... | | | | | |

### 3b. When to Use Decision Tree

```
┌─ Where does the data come from?
│
├── External API?
│   └── {Factory Type for external API}
│
├── Local DB?
│   ├── Need HTTP ETag (conditional fetch)?
│   │   ├── Single entity? → {Type}
│   │   └── Aggregated? → {Type}
│   └── No ETag needed?
│       └── {Standard Factory Type}
│
└── Other?
    └── {Custom type if present}
```

---

## Step 4 — Write `knowledge_factory.md`

> ⚠️ ALL class names, packages, constants MUST be REAL values — no placeholders.
> ⚠️ EACH factory type MUST have **Factory Context Q&A** (5 questions).

### Output template:

````markdown
# Factory Knowledge

_Generated from codebase analysis — YYYY-MM-DD._
_Scoped to: {scope}._

---

## Overview & Comparison

### Factory Type Comparison
{Table comparing ALL factory types: base class, interface, data source, cache, constructor, when to use}

### When to Use Which Factory?
{Decision tree from Step 3b}

### Count Summary
{Table: type | count | use case}

---

## Type 1: {Real name — e.g., BaseCrudDataFactory / BaseClientDataFactory}

### 1. Dùng để làm gì? (Purpose)
{Định nghĩa rõ factory này giải quyết bài toán gì. Ví dụ: "Lấy dữ liệu từ external API" hoặc "Lưu dữ liệu xuống DB"}
> {Nêu lý do vì sao dùng factory này thay vì gọi trực tiếp Repo/Client: cache, mapping, etc.}

### 2. Khi nào dùng? (When to use)
{Các Use-case cụ thể BẮT BUỘC dùng base class này. Và khi nào KHÔNG ĐƯỢC DÙNG}

### 3. Dùng như thế nào? (How to use)

#### 3.1. Các phương thức bắt buộc (Must Override)
| Method | Signature | Purpose | Can return null? |
|---|---|---|---|

#### 3.2. Cấu hình Constructor & Generic Types
{Generic type order: exact types required}
{REAL code snippet: Cách inject IVnpayCacheManager, IAgwClient hoặc Repository vào constructor}

#### 3.3. Hierarchy (Sơ đồ kế thừa)
{Diagram từ Interface → Base Class → Implementation}

#### 3.4. Interface Rule — CRITICAL
{Quy tắc tạo domain interface:
- PHẢI extends base interface (IClientDataFactory/IDataFactory/...)
- Generic params: <Model, Filter>
- Body: thường rỗng, KHÔNG tạo custom method
- REAL code snippet: ✅ đúng vs ❌ sai}

#### 3.5. Handler ↔ Factory Call Pattern
{Handler gọi factory qua method nào?
- Method name + signature từ base interface
- REAL code snippet từ handler đang dùng
- ✅ đúng: factory.getData(filter)
- ❌ sai: factory.getDetail(filter) (custom method)}

#### 3.6. Code Template chuẩn
{Template class code sử dụng đúng Generic, Constructor và Override methods}
{Template interface code extends base interface}
{Template handler code gọi factory đúng cách}

#### 3.7. Danh sách các Factory đang dùng (Instances Found)
{Table: ClassName | Interface | Types | Service | Key overrides | Cache config}

---

## Type 2: {Real name}
{Lặp lại định dạng Dùng để làm gì / Khi nào dùng / Dùng như thế nào ở trên}

---

## Type N: {Real name}
{Lặp lại cùng định dạng}

---

## Cache Invalidation Mechanism

### Subscriber/Listener
{Class name, topic, bean discovery, guard logic}

### Bean Naming Rule
{Rule: @Component("XXX") → XXX = ? → subscriber lookup}

### Constants
{File locations, current values}

### Pattern
{REAL code snippet}

---

## ETag Pattern (if present)

### ETag Flow
{Client → Handler → ETag check → data or null}

### ETag Types
{Table: ETagType enum values}

### Handler Pattern
{REAL code snippet}

---

## Caching Mechanism Summary

### Auto-Caching Flow Per Factory Type
{Table: Factory | Read Flow | Write Flow | Invalidation}

### CacheConfigFactory Reference
{REAL code snippet — TTL, async, enable flags}

---

## Key Rules & Decisions
{Numbered list — MUST-follow rules, derived from source}

---

## Cross-References
{Links to related knowledge files}
````

---

## Step 5 — Self-Verification

1. [ ] ALL factory types documented with Q&A
2. [ ] Cache invalidation mechanism documented
3. [ ] Comparison table + decision tree present
4. [ ] ALL factory instances listed
5. [ ] NO `{placeholder}` remains
6. [ ] Code snippets from actual source
7. [ ] ETag flow documented (if present)
8. [ ] Bean naming rule documented
9. [ ] Constructor patterns per type documented
10. [ ] Generic type order documented per type
11. [ ] **Interface Rule documented**: domain interface extends base interface, body rỗng, KHÔNG custom method
12. [ ] **Handler ↔ Factory call pattern documented**: handler gọi `getData(filter)` KHÔNG gọi custom method
13. [ ] **Base interface methods listed**: tên + signature cho mỗi base interface

---

## Guardrails

- DO NOT assume factory names are "CrudDataFactory"/"ClientDataFactory" — find actual names
- DO NOT invent factory instances — only document what FOUND
- DO NOT leave any `{placeholder}` in output
- If a factory type returns 0 results → write "NOT FOUND in this source"
- Code snippets from actual source (shortened OK, invented NOT OK)
- Factory Context Q&A answers MUST derive from source — not generic
- Output MUST include ALL instances found
- **DO NOT confuse `BaseCrudDataFactory` with `BaseClientDataFactory`** — they have different constructors, different required methods, different data sources
- **`BaseClientDataFactory` MUST document**: `callClient()`, `convertToModel(response, filter)`, `makeKey()` — these are the critical methods that distinguish it from `BaseCrudDataFactory`
- **Filter pattern MUST be documented**: base filter class, builder helpers like `fromBaseSessionRequest()`, feature-specific filters extending base
