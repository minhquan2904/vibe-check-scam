# Delta Specification

## Impact Scope
| Component | Before | After | Impact |
|-----------|--------|-------|--------|
| API Routing | No `/health` routes | Exposes `/health` and `/health/ready` | Low (additive only) |
| Authorization | All endpoints subject to global config | `/health` prefix is explicitly excluded via `[AllowAnonymous]` | Low (isolated to new endpoints) |
| Configuration | No health config | Binds `HealthCheck` section in `appsettings.json` | Low (additive only) |
| Application Boot | No background DI tasks | Registers memory cache, dependency checks, and health service in DI | Low (minor memory/startup addition) |

## Behavioral Changes
| # | Area | Previous | Current | Type |
|---|------|----------|---------|------|
| 1 | External Monitoring | Returning 404 or requiring custom ping endpoints | Returning standardized JSON health status (200/503) | Feature Addition |
| 2 | Database Resiliency | No active connectivity tracking | Pings database via `SELECT 1` with configurable timeout (`DependencyTimeoutMs`) | Feature Addition |
| 3 | Diagnostics | Uptime and version not exposed via API | Uptime tracked via static internal Stopwatch and exposed on health endpoints | Feature Addition |
