# Planning Feature Rules

> **PRJ-10** — Rules for generating planning artifacts: Specs and Design documents.

---

## 1. General Principles

- Planning artifacts **MUST** be derived from `proposal.md` and `srs.md`. Do not invent requirements.
- All requirements **MUST** use RFC 2119 keywords: **MUST**, **SHALL**, **SHOULD**, **MAY**.
- **NEVER** include implementation details (class names, method names, SQL) in `specs/`.
- **NEVER** lose any requirement from `proposal.md` or `srs.md`. Every point must be traceable.
- Delta format only — specs describe **WHAT CHANGES**: `ADDED`, `MODIFIED`, `REMOVED`.

---

## 2. Specs Rules (`specs/**/*.md`)

### 2.1 File Structure

- One `spec.md` file per domain/module affected.
- Path: `specs/<domain>/spec.md`
- Domain name: kebab-case, matching the capability name from `proposal.md`.

### 2.2 Requirement Format

```
### Requirement: <Name>
<Description using SHALL/MUST keywords>

#### Scenario: <Name>
- **WHEN** <condition>
- **THEN** <expected outcome>
```

### 2.3 Rules

| Rule | Detail |
|------|--------|
| Every requirement MUST have ≥ 1 Scenario | No bare requirements without test scenario |
| Use exactly `####` for Scenario headers | `###` or bullets will NOT be parsed |
| Scenarios must be testable | Each Scenario = 1 potential test case |
| MODIFIED requirements: copy FULL block | Partial content will lose detail at archive |
| REMOVED requirements: provide Reason + Migration | Never silently remove |

### 2.4 Forbidden in Specs

- ❌ Class names, method signatures
- ❌ Database table names or column names
- ❌ Framework-specific details (ASP.NET, Angular, Oracle)
- ❌ Implementation approaches or technology choices

---

## 3. Design Rules (`design.md`)

### 3.1 Mandatory Sections

| Section | Content |
|---------|---------|
| Feature Profile | Mode, Flow Type, Auth, Layers |
| API Contracts | Method, Path, Auth, Request/Response DTOs |
| Component Design | Controller → Service → Repository layers |
| Entity Design | DB tables (if any), field names, types |
| Configuration | Settings keys, defaults, environment |
| Error Codes | HTTP code, business code, message |

### 3.2 Feature Profile (MUST match proposal)

Feature Profile is **LOCKED** once set in `proposal.md`. Design MUST NOT change:
- `Mode` (Query / Command / Hybrid)
- `Flow Type`
- `Auth` (EXCLUDED / REQUIRED)

### 3.3 API Contract Rules

- Request/Response DTOs must be named per PRJ-06 (validation) and PRJ-05 (response) conventions.
- Health/Public endpoints: explicitly state `[AllowAnonymous]` in design.
- Timeout configurations MUST be documented (key + default value).

### 3.4 Error Code Rules

- Every error scenario from `srs.md` MUST map to an HTTP code + business message.
- NEVER design an endpoint that returns raw exception detail to the client.
- Use HTTP 503 only when critical dependencies are DOWN (per SRS definition).

### 3.5 Cross-Check Before Finalizing Design

- [ ] All APIs from `srs.md` are covered
- [ ] All error scenarios from `srs.md` are handled
- [ ] Feature Profile matches `proposal.md`
- [ ] No auth-bypass risk (public endpoints explicitly excluded)
- [ ] Response format consistent with PRJ-05

---

## 4. General Planning Guardrails

| Guardian | Rule |
|----------|------|
| **Completeness** | Every SRS requirement → at least 1 spec requirement |
| **Consistency** | API paths in design must match SRS exactly |
| **Traceability** | Each spec requirement cites its SRS source |
| **Security** | Public endpoints documented and justified |
| **Testability** | Every spec scenario is independently verifiable |
