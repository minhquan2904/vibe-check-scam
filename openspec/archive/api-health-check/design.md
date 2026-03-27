# Design: API Health Check

**Change**: `api-health-check`
**Flow Type**: Query
**Type**: NEWBUILD
**Feature Profile** (🔒 Locked):

| Dimension | Value |
|-----------|-------|
| Mode | Query (read-only, no state mutation) |
| Auth | EXCLUDED — public endpoints |
| Layer | Controller → Service (no Repository) |
| Entity | None (no DB write, no new table) |

---

## Context

The backend service currently has no standardized liveness/readiness endpoint. Load balancers, CI/CD pipelines, and monitoring tools (Prometheus, Grafana, Kubernetes probes) require a known, authenticated-free route to verify service health without interfering with business traffic.

The existing auth middleware applies JWT validation to all routes. The `/health*` routes must be carved out explicitly. No equivalent health check controller exists in the current codebase.

---

## Goals / Non-Goals

**Goals:**
- Add `GET /health` (liveness — fast, no dependency check)
- Add `GET /health/ready` (readiness — checks DB + cache with timeout)
- Exclude `/health*` from JWT middleware
- Support configurable dependency timeout and version string
- Return consistent JSON (status, timestamp, version, uptime, dependencies)
- Produce no DB writes; DB interaction limited to `SELECT 1` ping

**Non-Goals:**
- Integration with ASP.NET Core's built-in `IHealthCheck` framework (overkill for this scope; custom lightweight implementation is preferred for control over response shape)
- Detailed tracing or distributed health metrics (Prometheus exporters)
- External API dependency checks (not in scope per proposal §5)
- Authentication on health endpoints (explicitly public)

---

## Decisions

### Decision 1: Custom implementation over `Microsoft.Extensions.Diagnostics.HealthChecks`

**Choice**: Implement a thin `HealthCheckService` + `HealthCheckController` manually rather than using the built-in `AddHealthChecks()` framework.

**Rationale**:
- The built-in framework's response shape (`HealthReport`) does not match the required JSON structure (`status`, `dependencies.<name>.status`, `responseTimeMs`). Custom serialization would be required regardless.
- The project's existing architecture uses a `Controller → Service` pattern. Introducing a separate middleware pipeline for health checks breaks consistency.
- The feature scope is simple (2 checks: DB + optional cache). The built-in framework adds complexity (registrations, UI, webhook) that is unnecessary.

**Alternative considered**: `AddHealthChecks()` + custom `IHealthCheckResponseWriter` — rejected because it couples to middleware ordering and requires additional DI wiring with no net simplification.

---

### Decision 2: `SELECT 1` for database liveness ping

**Choice**: Execute `SELECT 1` via `DbConnection.ExecuteScalarAsync()` with a cancellation token at `DependencyTimeoutMs`.

**Rationale**:
- Minimal query: no table scans, no locks, no side effects.
- Works on Oracle, SQL Server, and PostgreSQL.
- Respects the `DependencyTimeoutMs` configuration key.

**Alternative considered**: `DbConnection.OpenAsync()` only — rejected because an already-open connection pool can succeed even if the DB is processing hangs.

---

### Decision 3: `IMemoryCache.TryGetValue` as cache liveness ping

**Choice**: Attempt a lightweight read from `IMemoryCache` with a sentinel key to verify the cache instance is responsive.

**Rationale**:
- `IMemoryCache` is in-process, so a simple `TryGetValue` call is sufficient to prove the cache object is healthy.
- If the project later integrates a distributed cache (Redis), `CacheHealthCheck` can be swapped to `IDistributedCache.GetAsync` without changing the controller or aggregation logic.

---

### Decision 4: Auth exclusion via `[AllowAnonymous]` on controller + middleware route filter

**Choice**: Apply `[AllowAnonymous]` on `HealthCheckController` AND configure the JWT middleware to skip `/health*` routes.

**Rationale**:
- `[AllowAnonymous]` alone is insufficient if a custom auth middleware runs before MVC attribute evaluation.
- Double-layer exclusion (attribute + middleware route skip) is the safest approach for public endpoints that must never return 401.
- Configuration key `HealthCheck:ExcludeAuthRoutes` allows ops to adjust without code changes.

---

## API Design

### `GET /health` — Liveness

| Field | Value |
|-------|-------|
| Method | `GET` |
| Path | `/health` |
| Authorization | `[AllowAnonymous]` |
| Request Body | — |

**Response 200 OK**:
```json
{
  "status": "UP",
  "timestamp": "2026-03-27T13:30:00+07:00",
  "version": "1.0.0",
  "uptime": 3600
}
```

---

### `GET /health/ready` — Readiness

| Field | Value |
|-------|-------|
| Method | `GET` |
| Path | `/health/ready` |
| Authorization | `[AllowAnonymous]` |
| Request Body | — |

**Response 200 OK (UP or DEGRADED)**:
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

**Response 503 Service Unavailable (DOWN)**:
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

## Component Design

```
GET /health
  └─ HealthCheckController.Liveness()
       └─ Returns HealthStatusResponse { status, timestamp, version, uptime }
       ← 200 OK

GET /health/ready
  └─ HealthCheckController.Readiness()
       └─ HealthCheckService.CheckAllAsync()
            ├─ DatabaseHealthCheck.PingAsync()  [CancellationToken at DependencyTimeoutMs]
            ├─ CacheHealthCheck.PingAsync()     [CancellationToken at DependencyTimeoutMs]
            └─ Aggregate → OverallStatus
       ← 200 OK (UP / DEGRADED)
       ← 503   (DOWN — database critical dep DOWN)
```

**Classes**:

| Class | Role |
|-------|------|
| `HealthCheckController` | Thin controller, 2 actions: `Liveness()` and `Readiness()` |
| `IHealthCheckService` / `HealthCheckService` | Orchestrates all dependency checks, computes aggregate status |
| `IDependencyHealthCheck` | Interface: `string Name`, `Task<HealthEntry> PingAsync(CancellationToken)` |
| `DatabaseHealthCheck` | Implements `IDependencyHealthCheck` — `SELECT 1` via `IDbConnectionFactory` (stubs UP if factory is not registered in DI) |
| `CacheHealthCheck` | Implements `IDependencyHealthCheck` — `TryGetValue` on `IMemoryCache` |
| `HealthStatusResponse` | Response DTO: `status`, `timestamp`, `version`, `uptime` |
| `HealthReadinessResponse` | Extends `HealthStatusResponse` + `dependencies: Dictionary<string, HealthEntry>` |
| `HealthEntry` | Sub-DTO: `status`, `responseTimeMs` |
| `HealthCheckOptions` | Configuration binding: `DependencyTimeoutMs`, `ExcludeAuthRoutes` |

---

## Configuration

| Key | Default | Source |
|-----|---------|--------|
| `HealthCheck:DependencyTimeoutMs` | `500` | `appsettings.json` / env |
| `HealthCheck:ExcludeAuthRoutes` | `["/health", "/health/ready"]` | `appsettings.json` |
| `AppVersion` | Assembly version | `appsettings.json` / env `APP_VERSION` |

---

## Error Codes

| HTTP Code | Business Code | When |
|-----------|--------------|------|
| `200` | — | Service UP or DEGRADED |
| `503` | `SERVICE_UNAVAILABLE` | Critical dependency (DB) is DOWN |
| `405` | `METHOD_NOT_ALLOWED` | Wrong HTTP method on health endpoints |
| `500` | `INTERNAL_ERROR` | Unhandled exception in health check logic |

---

## Risks / Trade-offs

| Risk | Mitigation |
|------|-----------|
| DB ping under high poll frequency (monitoring every 30s) adds DB load | `SELECT 1` is negligible; connection pool reuse keeps overhead minimal |
| Auth exclusion misconfiguration exposes business routes | Double-layer (attribute + middleware skip) + integration test case to verify public access |
| Version info leaks in production | Env-based control: verbose diagnostics only when `ASPNETCORE_ENVIRONMENT=Development` |
| Cache check always passes (in-process cache never unavailable) | Intentional — cache is non-critical; if Redis is introduced, implement `IDistributedCache` check instead |
| Unhandled exception exposes 500 + stack trace | Global exception handler + catch-all in `Readiness()` → returns 503 + logs internally |

---

## Migration Plan

1. Add `HealthCheckController` → `[Route("health")]`, `[AllowAnonymous]`
2. Register `IHealthCheckService`, `DatabaseHealthCheck`, `CacheHealthCheck` in DI (`AddScoped`)
3. Bind `HealthCheckOptions` from `appsettings.json`
4. Configure middleware: skip JWT validation for `/health` and `/health/ready`
5. Add smoke test: `GET /health` → 200 in integration test suite

**Rollback**: Controller removal + middleware config revert — no DB migration required.

---

## Open Questions

- [ ] Should the Swagger/OpenAPI spec include or exclude the `/health*` endpoints? (Recommend: exclude or mark `[ApiExplorerSettings(IgnoreApi = true)]` to avoid false alarm in API consumers)
- [ ] Is there a Redis instance in the current environment that should be checked, or is `IMemoryCache` the only cache?
