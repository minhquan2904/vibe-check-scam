# New API Endpoints

| # | Method | Path | Controller | Handler | Description |
|---|--------|------|------------|---------|-------------|
| 1 | `GET` | `/health` | `HealthCheckController` | `Liveness()` | Returns basic liveness status (`UP`), timestamp, application version, and uptime. No authentication required. |
| 2 | `GET` | `/health/ready` | `HealthCheckController` | `Readiness()` | Returns readiness status by pinging all registered dependencies (database, cache). Returns 200 OK (`UP`/`DEGRADED`) or 503 Service Unavailable (`DOWN`). No authentication required. |

### Endpoint 1: Liveness (`GET /health`)
- **Request Body**: None
- **Response Body**: `HealthStatusResponse` (`{"status": "UP", "timestamp": "...", "version": "...", "uptime": 123}`)
- **Authentication**: Excluded / AllowAnonymous
- **Purpose**: Fast, low-impact endpoint for load balancers or orchestrators (e.g., Kubernetes liveness probe) to verify the service process is running and can serve HTTP traffic.

### Endpoint 2: Readiness (`GET /health/ready`)
- **Request Body**: None
- **Response Body**: `HealthReadinessResponse` (Liveness payload + `{"dependencies": {"database": {"status": "UP", "responseTimeMs": 10}, "cache": {"status": "UP", "responseTimeMs": 2}}}`)
- **Authentication**: Excluded / AllowAnonymous
- **Purpose**: Deep health check for traffic routing decisions (e.g., Kubernetes readiness probe). Verifies the service can communicate with its required backend infrastructure.
