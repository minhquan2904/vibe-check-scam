---
name: entity-gen
description: Sinh Entity class từ DDL/SRS. Tham chiếu dotnet-entity-convention.md + dotnet-relationship-convention.md.
version: 1.0
---

# Entity Generation Sub-Skill

> **BẮT BUỘC đọc trước khi sinh code:**
>
> - `../dotnet-convention-checker/dotnet-entity-convention.md` — checklist E1–E10
> - `../dotnet-convention-checker/dotnet-relationship-convention.md` — checklist R1–R8

---

## Input

| Source              | Format                     | Example                             |
| ------------------- | -------------------------- | ----------------------------------- |
| DDL Script          | `CREATE TABLE OMNI_{NAME}` | `CREATE TABLE OMNI_BACKGROUND(...)` |
| SRS / Agent Handoff | Entity list + attributes   | `agent_handoff.md`                  |
| Existing Entity     | Reference pattern          | `BackgroundEntity.cs`               |

---

## Generation Rules

### 1. File Structure

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOBase.Domain;

[Table("{TABLE_NAME}")]
public class {Name}Entity : BaseFieldEntity
{
    [Key]
    [Column("{PK_COLUMN}")]
    public string {PkProperty} { get; set; }

    [Column("{COLUMN_NAME}")]
    public {Type} {PropertyName} { get; set; }

    // ... more properties
}
```

### 2. Mapping Rules

| DDL Element           | C# Convention                                  |
| --------------------- | ---------------------------------------------- |
| Table `OMNI_{NAME}`   | `[Table("OMNI_{NAME}")]`                       |
| Column `VARCHAR2`     | `string`                                       |
| Column `NUMBER`       | `int` / `long` / `decimal`                     |
| Column `NUMBER(1)`    | `int` (EBoolean pattern)                       |
| Column `TIMESTAMP(6)` | `DateTime?`                                    |
| Column `CLOB`         | `string`                                       |
| PK Column             | `[Key]` attribute                              |
| Column name           | `[Column("SNAKE_CASE")]` → PascalCase property |

### 3. Base Class Selection

| Scenario                                                                     | Base Class                  |
| ---------------------------------------------------------------------------- | --------------------------- |
| Có audit fields (CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, IsActive) | `BaseFieldEntity`           |
| Có thêm approval fields (Status, ApprovedBy, ApprovedDate)                   | `BaseFieldApprovableEntity` |

### 4. Naming

| Element   | Convention                      |
| --------- | ------------------------------- |
| Class     | `{Singular}Entity` (PascalCase) |
| File      | `{Name}Entity.cs`               |
| Namespace | `BOBase.Domain`                 |
| Folder    | `Entities/{Module}/`            |

---

## Output

- File: `${PROJECT_ROOT}/src/BOBase.Domain/Entities/{Module}/{Name}Entity.cs`
- PHẢI pass checklist E1–E10 trong `dotnet-entity-convention.md`

---

## Example

**Input DDL:**

```sql
CREATE TABLE OMNI_BACKGROUND (
    CODE VARCHAR2(20) NOT NULL,
    VI_NAME NVARCHAR2(200),
    EN_NAME NVARCHAR2(200),
    IMAGE_URL VARCHAR2(500),
    DISPLAY_ORDER NUMBER,
    IS_DEFAULT NUMBER(1) DEFAULT 0,
    IS_ACTIVE NUMBER(1) DEFAULT 1,
    CREATED_BY NUMBER DEFAULT 1,
    CREATED_DATE TIMESTAMP(6) DEFAULT sysdate,
    MODIFIED_BY NUMBER,
    MODIFIED_DATE TIMESTAMP(6)
);
```

**Output Entity:**

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOBase.Domain;

[Table("OMNI_BACKGROUND")]
public class BackgroundEntity : BaseFieldEntity
{
    [Key]
    [Column("CODE")]
    public string Code { get; set; }

    [Column("VI_NAME")]
    public string? ViName { get; set; }

    [Column("EN_NAME")]
    public string? EnName { get; set; }

    [Column("IMAGE_URL")]
    public string? ImageUrl { get; set; }

    [Column("DISPLAY_ORDER")]
    public int? DisplayOrder { get; set; }

    [Column("IS_DEFAULT")]
    public int? IsDefault { get; set; }
}
```

> **Note:** Audit fields (`IsActive`, `CreatedBy`, `CreatedDate`, `ModifiedBy`, `ModifiedDate`) đã có trong `BaseFieldEntity` → KHÔNG khai báo lại.
