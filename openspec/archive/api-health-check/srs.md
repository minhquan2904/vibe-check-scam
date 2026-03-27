# SRS: API Health Check

**Change**: `api-health-check`
**Flow Type**: Query
**Phase**: Phase 1 (Query — Phase 1 only)

---

## 1. Overview

Cung cấp 2 endpoint HTTP public (không cần auth) để kiểm tra trạng thái service:

| Path | Loại | Mô tả |
|------|------|-------|
| `GET /health` | Liveness | Service còn sống không? |
| `GET /health/ready` | Readiness | Service sẵn sàng nhận traffic không? (có check deps) |

---

## 2. Phase 1 — API Specification

### 2.1. Liveness Endpoint

**Request**

| Field | Value |
|-------|-------|
| Method | `GET` |
| Path | `/health` |
| Auth | ❌ None (public) |
| Query Params | — |
| Body | — |

**Response — 200 OK**

| Field | Type | Description |
|-------|------|-------------|
| `status` | `string` | `UP` \| `DOWN` |
| `timestamp` | `string (ISO 8601)` | Thời điểm trả response |
| `version` | `string` | Phiên bản service (từ env) |
| `uptime` | `long` | Số giây service đã chạy |

```json
{
  "status": "UP",
  "timestamp": "2026-03-27T13:30:00+07:00",
  "version": "1.0.0",
  "uptime": 3600
}
```

---

### 2.2. Readiness Endpoint

**Request**

| Field | Value |
|-------|-------|
| Method | `GET` |
| Path | `/health/ready` |
| Auth | ❌ None (public) |
| Query Params | — |
| Body | — |

**Response — 200 OK (all UP)**

| Field | Type | Description |
|-------|------|-------------|
| `status` | `string` | `UP` \| `DEGRADED` \| `DOWN` |
| `timestamp` | `string (ISO 8601)` | Thời điểm trả response |
| `version` | `string` | Phiên bản service |
| `uptime` | `long` | Số giây uptime |
| `dependencies` | `object` | Map tên dependency → HealthEntry |

**HealthEntry (per dependency)**

| Field | Type | Description |
|-------|------|-------------|
| `status` | `string` | `UP` \| `DOWN` \| `TIMEOUT` |
| `responseTimeMs` | `long` | Thời gian kiểm tra (ms) |

```json
{
  "status": "UP",
  "timestamp": "2026-03-27T13:30:00+07:00",
  "version": "1.0.0",
  "uptime": 3600,
  "dependencies": {
    "database": { "status": "UP", "responseTimeMs": 12 },
    "cache":    { "status": "UP", "responseTimeMs": 3 }
  }
}
```

**Response — 200 OK (DEGRADED — một dep DOWN)**

```json
{
  "status": "DEGRADED",
  "timestamp": "2026-03-27T13:30:00+07:00",
  "version": "1.0.0",
  "uptime": 3600,
  "dependencies": {
    "database": { "status": "UP",      "responseTimeMs": 12 },
    "cache":    { "status": "DOWN",    "responseTimeMs": 501 }
  }
}
```

**Response — 503 Service Unavailable (DOWN)**

```json
{
  "status": "DOWN",
  "timestamp": "2026-03-27T13:30:00+07:00",
  "version": "1.0.0",
  "uptime": 0,
  "dependencies": {
    "database": { "status": "DOWN", "responseTimeMs": 5001 }
  }
}
```

---

## 3. Processing Flow

```
GET /health
└─ HealthCheckController.Liveness()
   └─ return { status: UP, timestamp, version, uptime }
   ← 200 OK

GET /health/ready
└─ HealthCheckController.Readiness()
   └─ HealthCheckService.CheckAll()
      ├─ DatabaseHealthCheck.PingAsync()  [timeout: 500ms]
      ├─ CacheHealthCheck.PingAsync()     [timeout: 500ms]  (optional)
      └─ aggregate → OverallStatus
   ← 200 OK (status: UP / DEGRADED)
   ← 503    (status: DOWN — tất cả critical deps DOWN)
```

**Aggregate Rule**:
- `UP`: tất cả dependencies `UP`
- `DEGRADED`: ít nhất 1 dep `DOWN` / `TIMEOUT`, nhưng DB chính vẫn `UP`
- `DOWN`: DB chính `DOWN` → 503

---

## 4. Error Scenarios

| Scenario | Behavior |
|----------|----------|
| DB connection timeout (> 500ms) | dep status = `TIMEOUT`, overall = `DEGRADED` hoặc `DOWN` |
| Cache không khả dụng | dep status = `DOWN`, overall = `DEGRADED` (cache không critical) |
| Exception không xử lý được | 503 + log error, KHÔNG expose stack trace |
| Endpoint bị gọi với method khác (POST/PUT) | 405 Method Not Allowed |

---

## 5. Configuration (via appsettings / env)

| Key | Default | Description |
|-----|---------|-------------|
| `HealthCheck:DependencyTimeoutMs` | `500` | Timeout per dependency check (ms) |
| `HealthCheck:ExcludeAuthRoutes` | `["/health", "/health/ready"]` | Routes excluded from JWT middleware |
| `AppVersion` | from `Assembly` hoặc env `APP_VERSION` | Version trả về trong response |

---

## 6. Security Requirements

- Endpoint `/health` và `/health/ready` PHẢI được exclude khỏi `[Authorize]` middleware.
- KHÔNG trả về thông tin nhạy cảm (connection string, internal IPs, exception detail) trong production.
- Môi trường dev: có thể trả thêm DB server name, version.

---

## 7. Non-Functional Requirements

| Yêu cầu | Mức đề xuất |
|---------|------------|
| Response time `/health` | < 50ms |
| Response time `/health/ready` | < 600ms (timeout deps 500ms + overhead) |
| Không tạo session / cookie | ✅ |
| Thread-safe | ✅ |
| Không write DB | ✅ (read-only: `SELECT 1`) |

---

## 8. Aggregated Error Codes

| HTTP Code | Code | Message |
|-----------|------|---------|
| 200 | — | `UP` hoặc `DEGRADED` |
| 503 | `SERVICE_UNAVAILABLE` | Critical dependency không khả dụng |
| 405 | `METHOD_NOT_ALLOWED` | Sai HTTP method |
| 500 | `INTERNAL_ERROR` | Lỗi không xử lý được trong health check |
