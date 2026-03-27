# Request Validation Rules

## 1. Data Annotations Validation

Validate as many fields as possible at the DTO layer to verify the validity of data sent from the client.
Use **Data Annotations** on Request DTO properties to validate format and presence **before** reaching business logic.

### Common Annotations

| Annotation | Purpose | When to use |
|-----------|---------|-------------|
| `[Required]` | Field must not be null or empty | Required fields |
| `[MaxLength(N)]` | Maximum string length | Name, remark, description |
| `[MinLength(N)]` | Minimum string length | Code, password |
| `[StringLength(max, MinimumLength)]` | String length range | Fields with min/max |
| `[Range(min, max)]` | Numeric range constraint | Page size, quantity |
| `[RegularExpression(pattern)]` | Field must match a regex | Format validation |
| `[EmailAddress]` | Valid email format | Email fields |
| `[Compare("OtherProp")]` | Must match another property | Password confirmation |

### Rules

- All Request DTOs **must** extend `FilterRequest` (for search) or use flat DTO (for create/update)
- Controller **must** validate via `ModelState` — framework handles this automatically with `[ApiController]`
- Error messages should use predefined error codes
- **KHÔNG** dùng `IServiceProvider` — chỉ DI trực tiếp (constructor injection)

### Example: Request DTO

```csharp
public class CustomerFilterRequest : FilterRequest
{
    [MaxLength(50)]
    public string SearchText { get; set; }

    [MaxLength(20)]
    public string AccountNo { get; set; }

    [Range(1, 100)]
    public int? PageSize { get; set; }
}
```

### Example: Create/Update Request

```csharp
public class CustomerCreateRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    [RegularExpression(PatternConstants.ACCOUNT)]
    public string AccountNo { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }
}
```

---

## 2. PatternConstants

> **ALWAYS** use `PatternConstants.*` — NEVER hardcode regex in DTOs.

### Account & Identity

| Constant | Regex | Usage |
|----------|-------|-------|
| `ACCOUNT` | `^[a-zA-Z0-9]+$` | Account number |
| `CIF` | `^[0-9]+$` | Customer CIF number |
| `MOBILE` | `^[0-9]+$` | Phone number |
| `ID_NUMBER` | `^[a-zA-Z0-9]+$` | ID card / Passport |

### Amount & Numeric

| Constant | Regex | Usage |
|----------|-------|-------|
| `AMOUNT` | `^\\d+(\\.\\d+)?$` | Amount (≥ 0) |
| `AMOUNT_POSITIVE` | `^(?!0+(\\.0+)?$)\\d+(\\.\\d+)?$` | Amount (> 0) |
| `NUMBER` | `^[0-9,.]+$` | General numeric value |
| `CCY` | `^[a-zA-Z]+$` | Currency code (VND, USD) |

### Name & Text

| Constant | Regex | Usage |
|----------|-------|-------|
| `NAME` | `^[a-zA-Z0-9 ]+$` | Name without accents |
| `NAME_ACCENT` | `^[\\p{L}0-9 ]+$` | Name with Vietnamese accents |
| `REMARK` | `^[a-zA-Z0-9 ();:*=@!?/,._-]{1,500}$` | General remark / note |
| `DESCRIPTION` | `^[\\p{L}0-9 ,.;:!?()-]{1,1000}$` | Long description |

### Date & Time

| Constant | Regex | Usage |
|----------|-------|-------|
| `DATE_DD_MM_YYYY` | `^(0[1-9]\|[12][0-9]\|3[01])-(0[1-9]\|1[0-2])-\\d{4}$` | Date format dd-MM-yyyy |
| `DATE_YYYY_MM_DD` | `^\\d{4}-(0[1-9]\|1[0-2])-(0[1-9]\|[12][0-9]\|3[01])$` | Date format yyyy-MM-dd |

### Codes & Enums

| Constant | Regex | Usage |
|----------|-------|-------|
| `ALPHA_NUMERIC` | `^[a-zA-Z0-9]+$` | General alphanumeric code |
| `PRODUCT_CODE` | `^[a-zA-Z0-9_-]+$` | Product / service code |
| `STATUS_CODE` | `^[A-Z_]+$` | Status enum (ACTIVE, INACTIVE) |
| `BOOLEAN_YN` | `^[YN]$` | Y/N flag |
