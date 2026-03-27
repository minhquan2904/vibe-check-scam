# Tasks: API Health Check

**Change**: `api-health-check`
**Flow Type**: Query — NEWBUILD
**Scope**: Backend (.NET) only — no Entity, no Oracle, no Angular

---

## 1. Configuration & Options

- [x] 1.1 Create `HealthCheckOptions.cs` — bind from `appsettings.json` section `HealthCheck` (`DependencyTimeoutMs: int`, `ExcludeAuthRoutes: string[]`)
- [x] 1.2 Register `HealthCheckOptions` in DI via `services.Configure<HealthCheckOptions>(config.GetSection("HealthCheck"))`
- [x] 1.3 Add default config block to `appsettings.json`: `HealthCheck.DependencyTimeoutMs = 500`, `HealthCheck.ExcludeAuthRoutes = ["/health", "/health/ready"]`

## 2. Response DTOs

- [x] 2.1 Create `HealthEntry.cs` — sub-DTO with `string Status`, `long ResponseTimeMs`
- [x] 2.2 Create `HealthStatusResponse.cs` — `string Status`, `string Timestamp`, `string Version`, `long Uptime`
- [x] 2.3 Create `HealthReadinessResponse.cs` — extends `HealthStatusResponse` adding `Dictionary<string, HealthEntry> Dependencies`

## 3. Dependency Health Check Interface & Implementations

- [x] 3.1 Create `IDependencyHealthCheck.cs` — interface with `string Name { get; }` and `Task<HealthEntry> PingAsync(CancellationToken ct)`
- [x] 3.2 Create `DatabaseHealthCheck.cs` — implements `IDependencyHealthCheck`; executes `SELECT 1` via injected `IDbConnectionFactory` (or stubs UP if null) with cancellation token at `DependencyTimeoutMs`; returns `HealthEntry { Status = "UP"/"DOWN"/"TIMEOUT", ResponseTimeMs }`
- [x] 3.3 Create `CacheHealthCheck.cs` — implements `IDependencyHealthCheck`; attempts `IMemoryCache.TryGetValue` with sentinel key; returns `HealthEntry { Status = "UP"/"DOWN", ResponseTimeMs }`
- [x] 3.4 Register both checks in DI as `IEnumerable<IDependencyHealthCheck>` (or named registrations)

## 4. Health Check Service

- [x] 4.1 Create `IHealthCheckService.cs` — interface with `Task<HealthReadinessResponse> CheckAllAsync()`
- [x] 4.2 Create `HealthCheckService.cs` — implements `IHealthCheckService, IScoped`; iterates all `IDependencyHealthCheck`; aggregates `OverallStatus` per rules (UP / DEGRADED / DOWN); catches unhandled exceptions — logs internally, returns DOWN without stack trace
- [x] 4.3 Register `IHealthCheckService → HealthCheckService` in DI

## 5. Controller

- [x] 5.1 Create `HealthCheckController.cs` — `[ApiController]`, `[Route("health")]`, `[AllowAnonymous]`; does NOT extend `BaseController` (health responses use raw JSON, not `Response(...)` wrapper per PRJ-05 — health endpoints use their own contract)
- [x] 5.2 Implement `Liveness()` action — `[HttpGet("")]`; builds and returns `HealthStatusResponse` directly (200 OK); reads version from config/assembly, uptime from `IHostApplicationLifetime` or static `Stopwatch`
- [x] 5.3 Implement `Readiness()` action — `[HttpGet("ready")]`; calls `IHealthCheckService.CheckAllAsync()`; returns 200 if UP/DEGRADED, 503 if DOWN; wraps unhandled exception → 503 + log

## 6. Auth Middleware Wiring

- [x] 6.1 Update JWT middleware configuration to skip routes matching `HealthCheck:ExcludeAuthRoutes` (e.g., using `app.UseWhen(ctx => !excludedRoutes.Contains(ctx.Request.Path), branch => branch.UseAuthentication().UseAuthorization())`)
- [x] 6.2 Verify: `GET /health` without Authorization header → 200 (not 401)

## 7. Verification

- [ ] 7.1 Manual smoke test: `GET /health` → 200, body has `status`, `timestamp`, `version`, `uptime`
- [ ] 7.2 Manual smoke test: `GET /health/ready` → 200 or 503 depending on DB state
- [ ] 7.3 Manual test: `POST /health` → 405 Method Not Allowed
- [ ] 7.4 Manual test: `GET /health` without auth header → 200 (not 401)
- [ ] 7.5 Confirm no stack trace in error response when exception occurs

