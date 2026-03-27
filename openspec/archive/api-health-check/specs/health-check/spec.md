# Spec: Health Check

**Change**: `api-health-check`
**Domain**: `health-check`
**Source**: `proposal.md §4`, `srs.md §2`, `srs.md §3`, `srs.md §4`, `srs.md §7`, `srs.md §8`

---

## ADDED Requirements

### Requirement: Liveness endpoint returns service status

The service SHALL expose a `GET /health` endpoint that returns the current operational status of the service without checking dependencies.

> **Source**: proposal.md §4, srs.md §2.1

#### Scenario: Service is running and healthy

- **WHEN** a client sends `GET /health`
- **THEN** the system returns HTTP 200 with `status: "UP"`, `timestamp` (ISO 8601), `version`, and `uptime` (seconds)

#### Scenario: Liveness endpoint requires no authentication

- **WHEN** a client sends `GET /health` without any Authorization header
- **THEN** the system returns HTTP 200 (request is not rejected by auth middleware)

#### Scenario: Liveness endpoint rejects wrong HTTP method

- **WHEN** a client sends `POST /health` or `PUT /health`
- **THEN** the system returns HTTP 405 Method Not Allowed

---

### Requirement: Readiness endpoint checks all critical dependencies

The service SHALL expose a `GET /health/ready` endpoint that checks each registered dependency and returns an aggregated readiness status.

> **Source**: proposal.md §4, srs.md §2.2, srs.md §3

#### Scenario: All dependencies are UP

- **WHEN** a client sends `GET /health/ready` and all dependencies respond within timeout
- **THEN** the system returns HTTP 200 with `status: "UP"` and per-dependency `status: "UP"` and `responseTimeMs`

#### Scenario: Non-critical dependency (cache) is DOWN

- **WHEN** a client sends `GET /health/ready` and cache is unavailable but database is UP
- **THEN** the system returns HTTP 200 with `status: "DEGRADED"` and cache entry showing `status: "DOWN"` or `status: "TIMEOUT"`

#### Scenario: Critical dependency (database) is DOWN

- **WHEN** a client sends `GET /health/ready` and database is unreachable
- **THEN** the system returns HTTP 503 with `status: "DOWN"` and database entry showing `status: "DOWN"`

#### Scenario: Readiness endpoint requires no authentication

- **WHEN** a client sends `GET /health/ready` without any Authorization header
- **THEN** the system returns HTTP 200 or 503 (not 401 or 403)

---

### Requirement: Dependency check timeout enforced

Each dependency check SHALL complete within a configurable timeout. If the check exceeds the timeout, the dependency status MUST be set to `TIMEOUT`.

> **Source**: srs.md §3, srs.md §4, srs.md §5

#### Scenario: Database check exceeds timeout

- **WHEN** the database does not respond within `HealthCheck:DependencyTimeoutMs` milliseconds
- **THEN** the database dependency entry shows `status: "TIMEOUT"` and the overall status is `DEGRADED` or `DOWN` depending on criticality

#### Scenario: Default timeout is 500ms

- **WHEN** no timeout is configured in app settings
- **THEN** the system uses 500ms as the default per-dependency timeout

---

### Requirement: Aggregate status computed from individual dependency statuses

The overall `status` returned in `/health/ready` SHALL be computed using the following aggregation rules:

- `UP`: ALL dependencies are `UP`
- `DEGRADED`: At least one non-critical dependency is `DOWN`/`TIMEOUT` but the database is `UP`
- `DOWN`: The database (critical dependency) is `DOWN`

> **Source**: srs.md §3

#### Scenario: All deps UP → overall UP

- **WHEN** all registered dependencies return `status: "UP"`
- **THEN** the response `status` is `"UP"` and HTTP 200

#### Scenario: Cache DOWN, DB UP → overall DEGRADED

- **WHEN** cache returns `status: "DOWN"` and database returns `status: "UP"`
- **THEN** the response `status` is `"DEGRADED"` and HTTP 200

#### Scenario: DB DOWN → overall DOWN

- **WHEN** database returns `status: "DOWN"`
- **THEN** the response `status` is `"DOWN"` and HTTP 503

---

### Requirement: Service version and uptime included in all health responses

Both `/health` and `/health/ready` SHALL include `version` (from configuration or assembly) and `uptime` (seconds since service start) in every response.

> **Source**: srs.md §2.1, srs.md §2.2

#### Scenario: Version from app settings or assembly info

- **WHEN** `APP_VERSION` environment variable is set
- **THEN** the `version` field reflects that value

#### Scenario: Uptime increases over time

- **WHEN** the service has been running for N seconds
- **THEN** the `uptime` field returns a value ≥ N

---

### Requirement: Unhandled exceptions in health check do not expose internal details

If an unexpected error occurs within the health check logic, the system SHALL return HTTP 503 and log the error internally, without exposing stack traces or internal system information.

> **Source**: srs.md §4, srs.md §6

#### Scenario: Unhandled exception during dependency check

- **WHEN** an unexpected exception is thrown during a dependency ping
- **THEN** the system returns HTTP 503 with `status: "DOWN"` and logs the full exception internally; no stack trace in the response

---

### Requirement: Response time targets met under normal conditions

The endpoints SHALL meet the following response time targets under normal load:

- `GET /health`: MUST complete in < 50ms
- `GET /health/ready`: SHOULD complete in < 600ms (allowing ≤ 500ms per dependency + overhead)

> **Source**: srs.md §7

#### Scenario: Liveness endpoint is fast

- **WHEN** a client sends `GET /health` under normal conditions
- **THEN** the system responds in < 50ms

#### Scenario: Readiness endpoint completes within timeout budget

- **WHEN** a client sends `GET /health/ready` and all dependencies respond within 500ms
- **THEN** the system responds in < 600ms total
