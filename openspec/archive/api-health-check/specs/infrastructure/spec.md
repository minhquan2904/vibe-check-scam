# Spec: Infrastructure

**Change**: `api-health-check`
**Domain**: `infrastructure`
**Source**: `proposal.md §3`, `srs.md §5`, `srs.md §6`

---

## ADDED Requirements

### Requirement: Health routes excluded from authentication middleware

The routes `/health` and `/health/ready` SHALL be excluded from the JWT authentication pipeline. Requests to these routes MUST NOT require an `Authorization` header.

> **Source**: proposal.md §3, srs.md §6

#### Scenario: Request without Authorization header is accepted

- **WHEN** a monitoring client sends `GET /health` without any credentials
- **THEN** the system processes the request and returns a health response (not 401 Unauthorized)

#### Scenario: Auth exclusion configured via app settings

- **WHEN** `HealthCheck:ExcludeAuthRoutes` is set to `["/health", "/health/ready"]` in configuration
- **THEN** those routes bypass the JWT middleware

---

### Requirement: Health check timeout configurable via app settings

The per-dependency timeout duration SHALL be configurable through `HealthCheck:DependencyTimeoutMs`. If not set, the default is 500ms.

> **Source**: srs.md §5

#### Scenario: Custom timeout from configuration

- **WHEN** `HealthCheck:DependencyTimeoutMs` is set to `300` in `appsettings.json`
- **THEN** each dependency check uses a 300ms timeout

#### Scenario: Default timeout when key is absent

- **WHEN** `HealthCheck:DependencyTimeoutMs` is not present in configuration
- **THEN** the system uses 500ms as the default timeout

---

### Requirement: Service version configurable via environment variable

The `version` field in health responses SHALL be sourced from the `APP_VERSION` environment variable when set, falling back to the assembly version.

> **Source**: srs.md §5

#### Scenario: APP_VERSION environment variable is set

- **WHEN** `APP_VERSION=1.2.3` is set in the environment
- **THEN** all health responses include `"version": "1.2.3"`

#### Scenario: APP_VERSION not set — uses assembly version

- **WHEN** `APP_VERSION` is not set
- **THEN** all health responses include the version string from the running assembly

---

### Requirement: Sensitive information NOT returned in production health responses

In production environments, health responses SHALL NOT include database connection strings, internal IP addresses, server hostnames, or detailed exception messages.

> **Source**: srs.md §6

#### Scenario: Production response is sanitized

- **WHEN** the service runs in production mode and `GET /health/ready` is called
- **THEN** the response contains only `status`, `timestamp`, `version`, `uptime`, and `dependencies.<name>.status` + `responseTimeMs` — no internal system details

#### Scenario: Dev environment may return extended info

- **WHEN** the service runs in development mode
- **THEN** the response MAY include additional diagnostic details (e.g., DB server name, version string)
