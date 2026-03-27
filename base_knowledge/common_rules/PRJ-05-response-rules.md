# Response Standards

## 1. Response Data — Only return what the client needs

### ⚠️ DO NOT return excessive data

| ❌ FORBIDDEN | ✅ CORRECT |
|-------------|-----------|
| Return entire Entity from database | Use Response DTO with only required fields |
| Return sensitive fields (password, PIN, internal token) | Remove completely from response |
| Return internal fields (CreatedBy, ModifiedDate, ...) | Only return if client actually needs them |
| Return raw external response | Map to appropriate Response DTO |

### Rules

- Response DTO **MUST** extend `BaseResponse` or use custom DTO
- Each API must have its own Response DTO — NEVER use Entity as response directly
- Use `[JsonIgnore]` for fields that should not be serialized
- Use Mapper class or manual mapping to convert from Entity to Response DTO
- Double-check: do not return any field that the client does not need to display
- Controller **MUST** use `return Response(...)` pattern

---

## 2. Required Response Structure

Every response returned to the client **MUST** follow the `return Response(...)` pattern from `BaseController`:

| Field | Type | Description | Required |
|-------|------|-------------|:--------:|
| `code` | int | HTTP-like status code (200 = success) | ✅ |
| `message` | string | User-friendly description | ✅ |
| `data` | object | Response data (null if error) | Per API |

### Success Response Example

```json
{
  "code": 200,
  "message": "Success",
  "data": {
    "code": "ABC123",
    "name": "Sample"
  }
}
```

### Pagination Response Example

```json
{
  "code": 200,
  "message": "Success",
  "data": {
    "items": [...],
    "totalCount": 100,
    "pageNumber": 1,
    "pageSize": 10
  }
}
```

### Error Response Example

```json
{
  "code": 400,
  "message": "Validation failed",
  "data": null
}
```

---

## 3. Error Handling — NEVER expose Exception details to client

### ⚠️ ABSOLUTELY DO NOT leak Exception information

| ❌ FORBIDDEN | ✅ CORRECT |
|-------------|-----------|
| Return stack trace to client | Return generic message, log error internally |
| Return Exception class name | Return predefined error code |
| Return SQL query, table name, column name | Return user-friendly message |
| Return server info (IP, port, version) | Only return `code` + `message` |

### Exception Handling Pattern in Controller

```csharp
[HttpPost("search")]
[Authorize(Roles = FunctionConst.FuncFeatureSearchId)]
public async Task<IActionResult> Search([FromBody] FeatureFilterRequest request)
{
    try
    {
        var result = await _featureService.SearchAsync(request);
        return Response(result);
    }
    catch (Exception ex)
    {
        // ✅ Log full details internally
        _logger.LogError(ex, "Search failed for {Request}", request);
        // ✅ Return generic message to client
        return Response(null, 500, "Yêu cầu không thể xử lý. Vui lòng thử lại sau.");
    }
}
```

---

## 4. Undefined Errors — Return default message

All errors without a specific mapped code **MUST** return the default message:

> **"Yêu cầu không thể xử lý. Vui lòng thử lại sau."**

- Each known business error must have its own error code
- NEVER return an empty response or one missing `code`/`message`

### Error Code Classification

| Category | Code | Description |
|----------|------|-------------|
| Success | `200` | Processed successfully |
| Client input error | `400` | Invalid input data |
| Unauthorized | `401` | Not authenticated |
| Forbidden | `403` | Not authorized (wrong FunctionConst role) |
| Not found | `404` | Resource not found |
| Business error | `422` | Business rule violation |
| System error | `500` | Unknown error / unhandled exception |
