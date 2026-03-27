# TODO & Uncover Items

## Statistics
| Type | Count |
|------|-------|
| TODO | 1 |
| FIXME | 0 |
| Edge Cases | 1 |

## Details
| # | Type | File | Line | Description |
|---|------|------|------|-------------|
| 1 | TODO | `HealthCheck/Checks/DatabaseHealthCheck.cs` | 10 | The `IDbConnectionFactory` is currently stubbed to return UP if not registered. This needs to be replaced with a real database context once the project integrates a database provider (e.g., EF Core, Dapper). |
| 2 | Edge Case | `HealthCheck/HealthCheckService.cs` | 46 | If an unhandled exception occurs during a dependency check (e.g., DNS timeout making a cache server unreachable), the service safely catches it and logs the error, returning `DOWN` for that specific dependency without crashing the readiness check. |
