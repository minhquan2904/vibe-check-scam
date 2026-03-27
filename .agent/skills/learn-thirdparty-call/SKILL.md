---
name: learn-thirdparty-call
description: Learn third-party / bank gateway call patterns from client_gateway module — scan, trace, and document the full lifecycle of an external API call into a structured knowledge file for LLM reuse.
---

## Purpose

Automatically scan the gateway module source code for calling third-party APIs, extract and document all patterns so the LLM can:
1. Understand the gateway architecture for calling external APIs (bank, partner, payment, etc.)
2. Know required base classes, interfaces, and annotations at each layer
3. Create new integrations (new gateway, new action, new domain client) following the correct structure
4. Map correctly from YAML config → Properties → Gateway → Action → Client → Request/Response
5. Handle errors, timeouts, logging, and monitoring for external calls

**Output filename**: `knowledge_thirdparty_call.md`
**Output directory**: Determined by caller (workflow passes `OUTPUT_DIR`). If called standalone, default to `base_knowledge/structures/propose/`.

---

## Input

User provides:
- **Gateway module path** (required): directory containing the third-party API call source
  - e.g., `client_gateway/api-bank-gw/`
- **Config file path** (optional): YAML endpoint configuration file
  - e.g., `config-service/src/main/resources/config/thirdparty-config-dev.yml`
- **Scope** (optional): specific domain name (e.g., `account`, `transaction`), defaults to scan ALL

---

## Step 1 — Find Entry Points

This is the most critical step. Do NOT assume structure — **scan the actual codebase** to discover patterns.

### 1a. Scan the gateway module root directory

```
list_dir({gateway_module_path})
```

Goal: determine:
- How many sub-modules (domains) exist?
- Is there a `base/` or `common/` module (containing shared infrastructure)?
- What is the actual directory structure?

### 1b. Find the Gateway Interface (API call entry point)

This is the main interface that all domain clients call into.

```
grep_search: "interface.*Gateway" in {gateway_module_path}
grep_search: "requestBank\|callApi\|callClient\|sendRequest\|execute" in {gateway_module_path}
```

**Look for**:
- Interface declaring a generic method: `<I, O> O requestBank(I request, ..., Class<O> responseType)`
- Implementation class: injects `RestTemplate/RestClientGatewayHandler` + `Properties`

**Read and record**:
- Interface name + implementation name
- Main method signature
- Injected dependencies (RestTemplate? WebClient? Feign? Custom handler?)
- URL building, headers, auth logic

> **CRITICAL WARNING**: This project may NOT use `@FeignClient`. 
> If a centralized gateway pattern (e.g., `I{Prefix}Gateway.requestBank()` where `{Prefix}` could be `Agw`, `Bgw`, `Vgw`, etc.) is found → document this is the ONLY way to call Bank.
> Do NOT mention "Feign" in the knowledge output unless `@FeignClient` annotations are actually found in source.
> The `{Prefix}` is project-specific — **discover it by scanning**, do not assume `Agw`.

### 1c. Find the Action Interface (endpoint definitions)

An Action defines each API endpoint to call.

```
grep_search: "interface.*Action" in {gateway_module_path}
grep_search: "getFunctionPath\|getPath\|getEndpoint\|getUrl" in {gateway_module_path}
```

**Look for**:
- Interface: typically has `getFunctionPath()` + `getPrefixErrorCode()` or similar
- Enum implementations: each domain has 1 enum, each entry = 1 API

**About prefixErrorCode**: This is an **auto-increment number**, each API is assigned a unique number for error identification. e.g.:
```java
GET_LIST_ACCOUNT("get-list-account", "01"),   // API #1
GET_DETAIL_ACCOUNT("get-detail-account", "02"), // API #2
GET_HISTORY_ACCOUNT("get-history-account", "03"), // API #3
```
→ When adding a new API, find the last number across ALL action enums → +1.

### 1d. Find Properties / Config binding

```
grep_search: "@ConfigurationProperties" in {gateway_module_path}
grep_search: "ClientProperties\|GatewayProperties" in {gateway_module_path}
```

**Look for**:
- Which class binds YAML config?
- What is the prefix? (e.g., `common.client.external.bank-gw`)
- Which class does it extend? (e.g., `ClientProperties.Scope`)
- Accessor methods: `getUri()`, `getProperties()`, `getSocketTimeout()`

### 1e. Find Request/Response base classes

```
grep_search: "class.*BaseRequest\|class.*BaseResponse" in {gateway_module_path}
```

**Look for**:
- Base request: common fields (requestId, sessionId, userId, ipAddress, ...)
- Builder pattern: `@SuperBuilder(toBuilder = true)` or lombok?
- Utility method: `buildBaseRequest()` auto-fill from security context?
- Base response: common fields (code, message, success, ...)

> **CRITICAL — Base Request**: If `buildBaseRequest()` exists, document EXACTLY which fields it auto-populates and from where.
> This prevents LLM from manually setting these fields in factory code.
> Example: `requestId` from `AuthenticationUtil.getCurrentRequestId()`, `sessionId` from `AuthenticationUtil.getCurrentSessionId()`, etc.
> Feature-specific gateway requests ONLY need to set their own fields; base fields are auto-filled by `.buildBaseRequest()`.

> **CRITICAL — Base Response**: Check if a base response class (e.g., `{Prefix}BaseResponse`) exists.
> ```
> grep_search: "class.*BaseResponse" in {gateway_module_path}
> grep_search: "extends.*BaseResponse" in {gateway_module_path}
> ```
> If found, document:
> - Exact class name and package path
> - Which fields are in the base (e.g., `code`, `message`, `success`, `requestId`, `responseTime`)
> - Feature-specific responses MUST `extends {Prefix}BaseResponse` — do NOT duplicate base fields
> **WHY**: If not documented, LLM will create standalone response classes with duplicated base fields.

### 1f. Find Error handling

```
grep_search: "ErrorCode\|ErrorData\|ThirdPartyError\|GatewayError" in {gateway_module_path}
grep_search: "defaultHttpResponseChecker\|responseChecker\|validateResponse" in {gateway_module_path}
```

**Look for**:
- Error code enum (e.g., CONNECTION_TIMEOUT, SERVER_ERROR, GATEWAY_TIMEOUT)
- Error data class (implements ThirdPartyErrorData?)
- Response checker util: validate HTTP status + business success logic
- Error code format string (e.g., `"AGW_ERROR_" + prefixErrorCode + responseCode`)

### 1g. Find Spring Configuration

```
grep_search: "@Configuration" in {gateway_module_path}
grep_search: "@Bean.*Gateway\|@Bean.*RestClient" in {gateway_module_path}
```

**Look for**:
- Configuration class: `@EnableConfigurationProperties` for which properties?
- Beans created: Gateway bean, RestClientHandler bean
- Dependency wiring: gateway depends on handler + properties

### 1h. Find Monitoring / Event Listener

```
grep_search: "EventListener\|MonitorEvent\|SystemMonitor" in {gateway_module_path}
```

**Look for**:
- Event listener class: `onCompleted()`, `onError()`
- Data written to monitor: source, responseCode, duration, user info
- Async or sync? Virtual thread?

---

## Step 2 — Read YAML Config (if available)

Read the provided YAML config file. Record:

### 2a. General structure

```yaml
<prefix>:
  <gateway-name>:
    uri: <base-url>
    socket-timeout: <ms>
    properties:
      <action-key-1>: <api-path-1>
      <action-key-2>: <api-path-2>
      key-auth: <credential>       # if present
```

### 2b. YAML ↔ Java Mapping

Verify:
- `<prefix>.<gateway-name>` must match `@ConfigurationProperties` prefix
- Each `<action-key>` must match a `functionPath` in the Action enum
- `uri` + `properties[action-key]` = full API URL

### 2c. List all actions by domain

Group action keys by domain (read YAML comments if available), create table:

| Domain | YAML Key | API Path | Action Enum |
|---|---|---|---|

---

## Step 3 — Scan Domain Modules

For EACH domain module found in Step 1a:

### 3a. Find Domain Client Interface + Implementation

```
grep_search: "interface.*Client" in {domain_module_path}
grep_search: "class.*Client.*implements" in {domain_module_path}
```

**Record**:
- Interface: name, method list
- Impl: `@Component`, injects the gateway interface (e.g., `I{Prefix}Gateway`)
- Each method = 1 call to `gateway.requestBank(request, Action.XXX, Response.class)`

> **CRITICAL RULE — MODIFY vs CREATE**:
> - When adding a new Bank API to an **existing domain** → **MODIFY** the existing `I{Prefix}XxxClient` interface + `{Prefix}XxxClient` implementation (add new method)
> - Do **NOT** create a new client class for each feature
> - Each domain has exactly **1 client interface + 1 implementation** → all APIs in that domain go into the same client
> - Also **MODIFY** the Action enum → add new constant

### 3a-extra. Trace from Service-Layer Factory to AGW Client

The connection between `BaseClientDataFactory` (in service module) and `I{Prefix}XxxClient` (in gateway module) is critical:

```
grep_search: "I{Prefix}.*Client\|I.*GwClient\|I.*Client" in {service_module_path}
grep_search: "callClient" in {service_module_path}
```

**Document**:
- Which factories inject which AGW client
- How `callClient(Filter)` builds the AGW request (`.buildBaseRequest()` + feature fields from filter)
- This is the bridge between `knowledge_factory.md` and `knowledge_thirdparty_call.md`

### 3b. Find Request/Response Models

```
find_by_name: Extensions=["java"], Pattern="*Request*" in {domain_module_path}
find_by_name: Extensions=["java"], Pattern="*Response*" in {domain_module_path}
find_by_name: Extensions=["java"], Pattern="*Model*" in {domain_module_path}
```

For each model, read and record:
- Extends which class? (AgwBaseRequest / AgwBaseResponse)
- Annotations used? (@SuperBuilder, @Getter, @Setter, ...)
- Domain-specific fields

### 3c. Find Domain-specific Enums

```
find_by_name: Extensions=["java"], SearchDirectory={domain_module_path}/enumerate or /enum
```

### 3d. Check for custom patterns

Check if the domain overrides/customizes anything beyond the base pattern:
- Custom headers?
- Custom auth (OAuth2, API key instead of Basic)?
- Custom response parsing?
- Post/pre processing hooks?

---

## Step 4 — Synthesize Full Call Chain

After reading everything, synthesize the actual call chain:

```
[Caller] Service layer / UseCase
  ↓ calls method on
[Domain Client] I{Prefix}{Domain}Client → {Prefix}{Domain}Client
  ↓ gateway.requestBank(request, action, responseType)
[Gateway] I{Prefix}Gateway → {Prefix}Gateway
  ↓ build URL = properties.getUri() + "/" + properties.getProperties().get(action.getFunctionPath())
  ↓ build headers (Content-Type, Accept, Authorization)
  ↓ build context (action, httpMethod, logging, socketTimeout)
[RestClientHandler] DefaultRestClientGatewayHandler
  ↓ restTemplate.exchange(url, POST, httpEntity, Object.class)
[External API] HTTP call → response
  ↓
[HTTP Checker] defaultHttpResponseChecker(response)
  ↓ validate HTTP status (200 OK? 502? no body?)
[Business Checker] defaultAgwGatewayResponseChecker(response, action)
  ↓ validate response.getSuccess() == true?
  ↓ false → throw VnpayInvalidException with formatted error code
[Converter] MapperUtil.convertValue(body, responseType)
  ↓
[Monitor] eventListener.onCompleted/onError → SystemMonitor.write()
  ↓
[Return] typed response object
```

Record **each step**, real class, real method, real parameters.

---

## Step 5 — Write `knowledge_thirdparty_call.md`

> ⚠️ **CRITICAL**: ALL class names, package paths, YAML keys MUST be **real values from the source** — no placeholders.

### Output file structure:

````markdown
# Third-Party Call Gateway Knowledge

_Generated from codebase analysis — YYYY-MM-DD._
_Source: `{actual_path}`._

---

## 1. Architecture Overview

### 1.1 Call Chain Diagram
{ASCII diagram from Step 4 — with REAL class names}

### 1.2 Module Structure
{Actual tree diagram from Step 1a}

---

## 2. YAML Configuration

### 2.1 Config Structure
{Actual YAML structure — NOT a template}

### 2.2 Config → Java Binding
{Table: YAML path → Java class → @ConfigurationProperties prefix → accessor}

### 2.3 All Actions by Domain
{Table: Domain | YAML Key | API Path | Action Enum | prefixErrorCode}

---

## 3. Base Infrastructure

### 3.1 Gateway Interface & Implementation
{REAL code snippet + explanation}
{URL building formula: uri + "/" + properties.get(action.getFunctionPath())}
{Header building: JSON + auth}

### 3.2 Action Interface & Enum Pattern
{REAL code snippet + explanation}
{RULE: prefixErrorCode auto-increments, each API = 1 number, find current max + 1 when adding new}
{Summary table of ALL action enums with current max prefixErrorCode}

### 3.3 Properties Binding
{@ConfigurationProperties + extends + accessor methods}

### 3.4 Request Base Model
{Fields + builder pattern + buildBaseRequest() method}

### 3.5 Response Base Model
{Fields: code, message, success, requestId, responseTime}

### 3.6 Context
{Fields: action, httpMethod, isLogging, requestId (from MDC), socketTimeout, eventListener}

---

## 4. Error Handling

### 4.1 HTTP-Level Errors
{Table: HTTP status → error code → behavior}

### 4.2 Business-Level Errors
{Logic: success=false → formatted error code}
{Format: "{ERROR_PREFIX}_{prefixErrorCode}{responseCode}"}

### 4.3 Error Code Enum
{All entries with ordinal + description + computed code}

### 4.4 Error Data
{ThirdPartyErrorData implementation}

### 4.5 Exception Types
{VnpayClientConnectionTimeoutException, VnpayInvalidException, generic Exception}

---

## 5. Monitoring & Logging

### 5.1 Event Listener
{onCompleted + onError → SystemMonitor fields}

### 5.2 Request/Response Logging
{LogContext.push(LogType.CALL_REST, ClientLog)}

---

## 6. Domain Modules Inventory

### 6.1 {Domain 1 — e.g., Account}
{Client interface + impl, actions (with error codes), request/response models, data models}

### 6.2 {Domain 2}
{Same structure}

...repeat for all domains...

---

## 7. How to Add New Integration

### 7.1. Dùng để làm gì? (Purpose)
{Mục đích sử dụng module Gateway này: Gọi API của đối tác nào? Phục vụ domain nào?}

### 7.2. Khi nào dùng phương pháp nào? (When to use)
| Use Case | Giải pháp (Method) |
|---|---|
| Gọi thêm API mới của đối tác CŨ | **MODIFY** domain client hiện có (Thêm Action mới) |
| Gọi API của hệ thống/đối tác MỚI | **CREATE** gateway mới hoàn toàn |

### 7.3. Dùng như thế nào? (How to use)

#### Thêm API mới vào Gateway CŨ (MODIFY)
{Step-by-step: 1. YAML config → 2. Thêm số thứ tự `prefixErrorCode` vào Action enum → 3. Kế thừa `{Prefix}BaseRequest/Response` → 4. Thêm hàm vào `I{Prefix}{Domain}Client`}

#### Tạo Gateway hoàn toàn MỚI (CREATE)
{Step-by-step: YAML block → Properties → Constants → ErrorCode → ErrorData → ModuleError → Checker → Context → EventListener → Gateway → Configuration → Action → Domain models + Client}

#### Quy tắc chặn lỗi (Guardrails & Constraints)
{Liệt kê những lỗi sai LLM hay mắc phải: KHÔNG dùng @FeignClient, MẤT requestId nếu thiếu buildBaseRequest, trùng field cha nếu không extends BaseResponse...}

---

## 8. Naming Conventions
{Table from real examples}

---

## 9. Key Rules
{Numbered list — MUST-follow rules}

---

## 10. Cross-References
{Links to related knowledge files: knowledge_architecture.md, knowledge_factory.md, ...}
````

---

## Step 6 — Self-Verification

After writing the file, verify:

1. [ ] ALL gateway module directories are listed
2. [ ] YAML config structure matches real config file
3. [ ] Call chain diagram uses REAL class names, not placeholders
4. [ ] ALL action enums documented with functionPath + prefixErrorCode
5. [ ] Max prefixErrorCode per gateway is identified
6. [ ] ALL domain client interfaces and implementations listed
7. [ ] NO `{placeholder}` remains — all replaced with real values
8. [ ] Code snippets from actual source (shortened but not invented)
9. [ ] "How to Add" covers both existing gateway + new gateway
10. [ ] Error handling chain is complete

---

## Guardrails

- DO NOT invent class names or YAML keys — only document what was FOUND
- DO NOT leave any `{placeholder}` in the output file
- DO NOT hardcode directory structure — scan the actual codebase first
- Code snippets MUST be from actual source (shortened OK, invented NOT OK)
- Output MUST reflect REAL project structure as-is
- If a domain module has custom patterns (different from base), document them clearly
- ALWAYS cross-reference related knowledge files if available
- **DO NOT mention `@FeignClient` unless actually found** in source — the gateway pattern may use a centralized gateway (e.g., `I{Prefix}Gateway.requestBank()`) instead
- **MUST document `buildBaseRequest()` auto-populated fields** — prevents LLM from manually setting requestId, sessionId, userId, etc.
- **MUST document base response class** (`{Prefix}BaseResponse`) — prevents LLM from duplicating base fields in feature responses
- **MUST emphasize MODIFY existing client** (add method) when adding new API to existing domain — do NOT create new client class
- **MUST document Action enum `prefixErrorCode` numbering** — find current max across ALL enums + 1 for new entries
- **MUST trace the connection** from `BaseClientDataFactory.callClient()` → `I{Prefix}XxxClient.method()` → `{Prefix}XxxClient.gateway.requestBank()` — this is the full call chain from service layer to external API
- **Gateway prefix (`{Prefix}`) is project-specific** — discover by scanning `interface.*Gateway` in gateway module, do NOT assume `Agw`
