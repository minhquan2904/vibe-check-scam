---
name: learn-dto-entity
description: Learn DTO, Entity, Repository patterns — scan how models are defined, how repositories declare custom queries, and how factories consume them. Document the full data-layer DNA for LLM reuse.
---

## Purpose

Automatically scan the source code, extract and document the **Data Layer DNA** (DTO/Entity/Repository/Factory wiring) so the LLM can:
1. Create new Entities with correct annotations, base class, and naming
2. Create new DTOs/Models with correct patterns (Builder, @Jacksonized, base class inheritance)
3. Define Repository methods following JPA conventions
4. Know how Factories call Repositories and convert Entity ↔ Model
5. Avoid structural mistakes, missing annotations, and incorrect generic types

**Output filename**: `knowledge_dto_entity.md`
**Output directory**: Determined by caller. Default: `base_knowledge/structures/propose/`.

---

## Input

- **Project root** (required): project root directory
- **Scope** (optional): specific service, defaults to scan ALL

---

## Step 1 — Find Entity Entry Points

### 1a. Find Base Entity Classes

```
grep_search: "abstract class Base.*Entity" in {project_root}
grep_search: "@MappedSuperclass" in {project_root}
grep_search: "class Base.*Entity" in {project_root}
```

**Record**:
- Base entity name, package location
- Built-in fields (id, createdDate, modifiedDate, status, ...)
- Annotations on base entity (`@MappedSuperclass`, `@EntityListeners`, `@Id`, `@GeneratedValue`)
- Builder pattern: `@SuperBuilder`? `@Builder`?

### 1b. Find all Concrete Entities

```
find_by_name: Pattern="*Entity.java", SearchDirectory={project_root}
```

Select **5-10 representative entities** (simple, complex, with relationships) and read:

Per entity document:
- Class name + table name (`@Table`)
- Which base entity does it extend?
- Required annotations: `@Entity`, `@Table`, `@Getter`, `@SuperBuilder`, `@NoArgsConstructor`, `@AllArgsConstructor`
- Column definitions: `@Column(name=...)`, nullable, length, unique
- Enum columns: `@Enumerated(EnumType.STRING)` or `@JdbcTypeCode(SqlTypes.NAMED_ENUM)`?
- Embedded objects: `@Embedded`, `@Embeddable`
- Relationships: `@ManyToOne`, `@OneToMany`, `@JoinColumn` (if present)
- JSON columns: `@JdbcTypeCode(SqlTypes.JSON)`, `@Type`

### 1c. Entity Naming Convention

```
grep_search: "@Table" + "name" in {project_root}
```

**Determine**:
- Table naming: snake_case? Prefix? Schema?
- Column naming: snake_case mapping strategy?
- Entity class suffix: always `Entity`?

---

## Step 2 — Find DTO / Model Entry Points

### 2a. Find Base Model / DTO Classes

```
grep_search: "abstract class Base.*Model" in {project_root}
grep_search: "class Base.*Model" in {project_root}
grep_search: "class Base.*Request" in {project_root}
grep_search: "class Base.*Response" in {project_root}
```

**Record per base class**:
- Name, package
- Built-in fields (customerId, channel, sessionId, ...)
- Builder pattern: `@SuperBuilder`? `@Builder(toBuilder = true)`?
- Annotations: `@Getter`, `@Setter`, `@NoArgsConstructor`, `@Jacksonized`

### 2b. Find Request / Response patterns

```
find_by_name: Pattern="*Request.java", SearchDirectory={project_root}, MaxDepth=10
find_by_name: Pattern="*Response.java", SearchDirectory={project_root}, MaxDepth=10
```

Select **5-10 representative request/response classes** and read:

Per DTO document:
- Class name
- Which base class does it extend?
- Required annotations
- Validation annotations: `@NotNull`, `@NotBlank`, `@Size`, `@Valid`
- Builder usage: `@Builder(toBuilder = true)` + `@NoArgsConstructor(access = AccessLevel.PROTECTED)`
- Naming pattern: `Get{Domain}Request`, `{Domain}Response`

### 2c. Find Model classes (domain model, intermediary)

```
find_by_name: Pattern="*Model.java", SearchDirectory={project_root}, MaxDepth=10
```

**Determine**:
- Model ≠ Entity ≠ Request ≠ Response? Is there an intermediary model?
- Where is the Model used? (Factory returns Model, Handler receives Model)
- Conversion pattern: Entity → Model (in Factory), Model → Response (in Handler)

---

## Step 3 — Find Repository Entry Points

### 3a. Find all Repository interfaces

```
grep_search: "extends.*Repository<" in {project_root}
grep_search: "extends ListCrudRepository" in {project_root}
grep_search: "extends JpaRepository" in {project_root}
grep_search: "extends MongoRepository" in {project_root}
grep_search: "@Repository" in {project_root}
```

### 3b. Analyze Custom Query Methods

Select **5-10 representative repositories** and read:

Per repository document:
- Interface name
- Extends: `ListCrudRepository<Entity, ID>` / `JpaRepository<Entity, ID>` / `MongoRepository<Entity, ID>`
- Spring Data JPA derived queries: `findByXxx`, `findAllByXxxAndYyy`, `existsByXxx`
- Custom `@Query` annotations (JPQL / native)
- `@Modifying` + `@Transactional` for update/delete queries
- Pagination: `Pageable` returning `Page<Entity>`
- Sort: `Sort` parameter
- Projections: interface-based or DTO-based?

### 3c. Repository Naming Convention

| Item | Convention | Example |
|---|---|---|
| Interface name | I{Domain}Repository? {Domain}Repository? | ? |
| Package | module/{domain}/repository/ | ? |
| Generic types | <Entity, ID type> | ? |

---

## Step 4 — Find Entity ↔ Model ↔ Factory Wiring

### 4a. How Factory calls Repository

```
grep_search: "crudRepository" in base factory classes
grep_search: "repository.find" in {project_root}
grep_search: "repository.save" in {project_root}
```

**Determine**:
- Does Factory call Repository directly or through base class methods?
- Does the base factory wrap CRUD methods: `getModel(id)`, `saveModel(model)`, `deleteModel(id)`?
- Custom queries: does Factory call `repository.findByXxx()` directly?

### 4b. Entity ↔ Model Conversion

```
grep_search: "convertToEntity" in {project_root}
grep_search: "convertToModel" in {project_root}
grep_search: "modelBuilder" in {project_root}
```

**Record**:
- Abstract method in Base Factory: `convertToEntity(model)` and `convertToModel(entity)`
- Builder pattern for conversion: `Entity.builder().xxx(model.getXxx()).build()`
- Is there a shared utility for conversion?

### 4c. Full Wiring Diagram

```
Handler
  → injects Factory (by Interface)
  → calls factory.getModel(id) / factory.saveModel(model)
     Factory
       → calls convertToEntity(model) / convertToModel(entity)
       → calls repository.findById(id) / repository.save(entity)
       → manages cache (get/put/evict)
```

---

## Step 5 — Write `knowledge_dto_entity.md`

> ⚠️ ALL class names, annotations MUST be REAL values from source — no placeholders.

### Output template:

````markdown
# DTO, Entity & Repository Knowledge

_Generated from codebase analysis — YYYY-MM-DD._

---

## Data Flow Overview
```
Client Request (DTO) → Handler → Factory → Repository → DB
                                    ↕ convert
                              Entity ↔ Model
```

## 1. Entity Patterns

### 1.1. Dùng để làm gì? (Purpose)
{Giải thích vai trò của Entity trong hệ thống (map DB table)}

### 1.2. Khi nào dùng? (When to use)
{Dùng khi định nghĩa bảng mới trong cơ sở dữ liệu. Mở rộng từ Base Entity nào tuỳ theo yêu cầu (có audit log hay không?)}

### 1.3. Dùng như thế nào? (How to use)

#### Base Entity Hierarchy
| Base Class | Package | Key Fields | Khi nào kế thừa class này? |
|---|---|---|---|

#### Quy tắc khởi tạo (Creation Rules)
{Các Annotation BẮT BUỘC: @Entity, @Table, @Getter, @SuperBuilder, etc.}

#### Cấu hình Column & Quan hệ
| Use Case (Khi nào dùng) | Annotation (Dùng như thế nào) | Example |
|---|---|---|
| Cột chuỗi (String) | @Column | ? |
| Cột Enum | @Enumerated / @JdbcTypeCode | ? |
| Cột JSON | @JdbcTypeCode(SqlTypes.JSON) | ? |

#### Code Template chuẩn
{REAL code snippet template}

---

## 2. DTO / Model Patterns

### 2.1. Dùng để làm gì? (Purpose)
{Phân biệt Request (nhận từ API), Response (trả về API), và Model (trung gian xử lý logic)}

### 2.2. Khi nào dùng loại nào? (When to use)
| Base Class | Khi nào dùng (Use Case) | Key Fields (Mặc định có sẵn) |
|---|---|---|
| BaseRequest | Nhận data từ Client | sessionId, channel... |
| BaseResponse | Trả kết quả cho Client | code, message... |
| BaseModel | Giao tiếp giữa Handler & Factory | ? |

### 2.3. Dùng như thế nào? (How to use)
#### DTO Creation Rules BẮT BUỘC
{Quy tắc @Jacksonized, Builder, AccessLevel.PROTECTED cho constructor}

#### Validation Annotations
{Cách dùng @NotNull, @NotBlank, @Valid}

#### Request / Response Code Template
{REAL code snippet template}

---

## 3. Repository Patterns

### 3.1. Dùng để làm gì? (Purpose)
{Cổng giao tiếp duy nhất xuống Database, thao tác trên Entity}

### 3.2. Khi nào dùng Interface nào? (When to use)
| Base Interface | Khi nào nên extends? |
|---|---|
| ListCrudRepository | Thao tác CRUD cơ bản |
| JpaRepository | Cần pagination, sorting |

### 3.3. Dùng như thế nào? (How to use)

#### Naming Conventions & Query Patterns
| Muốn làm gì? | Dùng như thế nào (Pattern) | Example |
|---|---|---|
| Tìm theo 1 field | `findByXxx` | `findByCustomerId(Long)` |
| Nhiều điều kiện | `findAllByXxxAndYyy` | ... |
| Query phức tạp | `@Query(...)` | ... |
| Update/Delete | `@Modifying @Transactional` | ... |

#### Repository Code Template
{REAL code snippet template}

---

## Entity ↔ Model Conversion

### Conversion Methods in Factory
| Method | Direction | Purpose |
|---|---|---|
| convertToModel | Entity → Model | Read from DB |
| convertToEntity | Model → Entity | Write to DB |
| updateConvertToEntity | Model + OldEntity → Entity | Update existing |

### Conversion Template
{REAL code snippet template}

---

## Full Wiring: Handler → Factory → Repository

### Wiring Diagram
{ASCII diagram}

### Example: Complete CRUD flow
{From Handler injecting Factory, calling method, Factory calling repository, converting}

---

## Key Rules
{Numbered list of MUST-follow rules}
````

---

## Step 6 — Self-Verification

1. [ ] ALL base entity classes documented
2. [ ] ALL base DTO/Model classes documented
3. [ ] Repository types and query patterns documented
4. [ ] Entity ↔ Model conversion documented
5. [ ] Full wiring diagram present
6. [ ] NO `{placeholder}` remains
7. [ ] Code snippets from actual source
8. [ ] Annotation reference table present
9. [ ] Templates for Entity, DTO, Repository present

---

## Guardrails

- DO NOT assume Entity/DTO naming — scan first
- DO NOT invent field names or annotations — only from source
- Cross-reference `knowledge_factory.md` for factory details
- Cross-reference `knowledge_handler.md` for handler-factory wiring
- Code snippets from actual source (shortened OK, invented NOT OK)
