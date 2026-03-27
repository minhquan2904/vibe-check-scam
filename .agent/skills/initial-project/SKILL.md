---
name: initial-project
description: Initialize project knowledge base including rules, standards, and visual system structures. Use when starting a new project or when the base knowledge system needs to be established.
---

Initialize the Base Knowledge system for the project by copying `base_knowledge/` from the git framework, then customizing and deriving standards from actual source code.

> **IMPORTANT**: `base_knowledge/` directory MUST already exist in the git framework (the same repository that provides `.agent/`). If not found, the flow **HALTS** immediately.

I'll set up:
- `base_knowledge/common_rules/` — Common project rules (from framework)
- `base_knowledge/standards/` — User-customized requirement list + coding standards reasoned from actual codebase
- `base_knowledge/structures/` — System overview + service/module architecture docs with Mermaid diagrams

---

**Input**: No arguments needed — automatically scans the current project.

**Steps**

## 0. Prerequisites (USER performs manually)

> **Before running `/initial-project`, the user MUST manually copy `base_knowledge/` from the git framework into the project root.**

```bash
# User performs this manually before running the workflow
cp -r <framework-path>/base_knowledge/ ./base_knowledge/
```

User should also customize `base_knowledge/standards/requirement_standards.md` before running:
- **Add standard**: Add a row to the Extend Requirements table (format: `<name>_standard.md` + description)
- **Remove standard**: Remove unnecessary rows for this project

## 1. Verify `base_knowledge/` Exists

Check that the `base_knowledge/` directory exists at the project root.

- **If found** → proceed to Step 2 (Read requirement_standards.md).
- **If NOT found → HALT immediately:**
  ```
  ❌ HALT: base_knowledge/ not found.
  Please copy base_knowledge/ from the git framework into the project root before running:
    cp -r <framework-path>/base_knowledge/ ./base_knowledge/
  
  Required structure:
  base_knowledge/
  ├── common_rules/
  ├── standards/
  │   └── requirement_standards.md
  └── structures/
  ```


## 2. Read `requirement_standards.md`

Read the file `base_knowledge/standards/requirement_standards.md` to determine the list of standards to derive.

This file is the **source of truth** for Step 3 — Agent parses the Base Requirements (mandatory) and Extend Requirements (optional) tables to know which standards to create.

## 3. Explore and Derive Project Standards (Explore Standards)

Use Explore Mode thinking to scan and analyze the current project codebase.

**Mission**: Read `base_knowledge/standards/requirement_standards.md` to get the list of standards to derive, then analyze the codebase accordingly.

**Step 3a — Read requirement_standards.md**:
- Parse the file to extract the list of Base Requirements and Extend Requirements.
- Base Requirements are MANDATORY — must be reasoned and output files created.
- Extend Requirements are OPTIONAL — reason and create output files only if applicable patterns are found in the codebase.

**Step 3b — Reason and create each standard file**:

For each standard listed in `requirement_standards.md`, scan the codebase to identify patterns:

- **`coding_standard.md`** (Base):
   - Scan class/method naming conventions, package structure, code formatting.
   - Check annotation usage patterns, import organization.
   - Check code comment patterns.
   - Write findings to `base_knowledge/standards/coding_standard.md`.

- **`logging_standard.md`** (Base):
   - Scan common log format usage (`log.info`, `log.error`, `log.debug`).
   - Check log message patterns (prefix usage, JSON serialization, etc.).
   - Check what data is logged vs. masked.
   - Write findings to `base_knowledge/standards/logging_standard.md`.

- **`error_handling_standard.md`** (Base):
   - Scan how exceptions are thrown (`UserException`, custom exceptions).
   - Check error code definitions (`Constants.ResCode`).
   - Check how error messages are retrieved (`CommonService.getMessage()`).
   - Check global exception handler patterns (`@ControllerAdvice`).
   - Write findings to `base_knowledge/standards/error_handling_standard.md`.

- **Extend Requirements** (if applicable):
   - For each item in the Extend Requirements table, check whether the codebase has sufficient patterns.
   - If clear patterns are found → create the corresponding standard file.
   - If insufficient patterns are found → skip that item.

**Each standards file should follow this template**:

```markdown
# <Standard Name> Standard

_Derived from actual codebase patterns on YYYY-MM-DD._

## Overview
[Brief description of what this standard covers]

## Patterns Found

### Pattern 1: [Name]
- **Where used**: [file paths / class names]
- **Frequency**: [how common — dominant / common / occasional]
- **Example**:
  ```java
  // actual code snippet from the project
  ```

### Pattern 2: [Name]
...

## Recommended Standard
[Based on the dominant patterns, the recommended standard is...]

## Anti-patterns Found
[Any inconsistencies or bad practices found that should be avoided]
```

**Note**: If the project is too large and requires a detailed plan for documenting all standards, suggest the user run `/opsx:propose init-base-standards` to create a Change Request.

## 4. Explore, Describe, and Visualize Structures (Explore & Visualize Structures)

Continue using Explore Mode thinking to scan the overall project architecture (check `docker-compose.yml`, `src/` directories, configuration files, etc.).

**Mission**: Map all system components/services, starting with a system overview.

### Step 4a — Create `system_overview.md` (MANDATORY — do this FIRST)

Before creating individual service files, create `base_knowledge/structures/system_overview.md` with the following sections:

```markdown
# System Overview

_Generated from codebase analysis on YYYY-MM-DD._

## Tech Stack

| Category       | Technology         | Version   | Notes                    |
|---------------|--------------------|-----------|--------------------------| 
| Language       | [e.g., Java]       | [version] | [from pom.xml/build.gradle] |
| Framework      | [e.g., Spring Boot]| [version] |                          |
| Build Tool     | [e.g., Maven]      | [version] |                          |
| Database       | [e.g., Oracle]     |           |                          |
| Cache          | [e.g., Redis]      |           |                          |
| Message Queue  | [e.g., Kafka]      |           |                          |
| ...            | ...                | ...       | ...                      |

## Architecture Pattern
[Monolith / Microservice / Modular Monolith — describe the pattern used]

## Infrastructure Dependencies
[List all external systems, databases, caches, message queues, etc.]

## Build & Deployment
[Build command, deployment target, CI/CD pipeline if identifiable]

## Service Map
[High-level list of all services/modules found in the project]
```

**How to extract tech stack**:
- Check `pom.xml` or `build.gradle` for language, framework, and dependency versions.
- Check `docker-compose.yml` for infrastructure (databases, caches, queues).
- Check configuration files (`application.yml`, `application.properties`) for connection details.
- **DO NOT hardcode or guess** — only include what's verifiable from the codebase.

### Step 4b — Identify and document individual services/modules

a. **Identify all services/modules**:
   - Scan for top-level directories that represent services (e.g., `authen-service/`, `transfer-service/`).
   - Check `docker-compose.yml` for service definitions.
   - Check build configurations (`pom.xml`, `build.gradle`).

b. **For each service/module, create `base_knowledge/structures/<service-name>.md`**:

   Each file MUST contain the following sections:

   ### Section 1 — Primary Responsibilities
   > What business problem does this service/module solve?

   ### Section 2 — Dependencies
   > - **Downstream** (this service calls): list of services/systems called.
   > - **Upstream** (calls this service): list of services/systems that call this one.
   > - **Infrastructure**: databases, caches, message queues used.

   ### Section 3 — Key Processing Flow
   > Pick the most important flow. To understand this flow, which core classes/files must a developer read? Write a brief description of how data flows through those classes.
   >
   > Example:
   > ```
   > Client Request
   >   → AuthController.process()
   >     → AuthService.authenticate()
   >       → JwtService.validateToken()
   >       → CacheManageService.getUserInfo()
   >     → AuthService.buildResponse()
   >   → Response
   > ```

   ### Section 4 — Visual Diagrams (MANDATORY)

   Must generate two types of diagrams:

   **Sequence Diagram** — illustrate data flow through the classes identified in Section 3:

   ````markdown
   ```mermaid
   sequenceDiagram
       participant Client
       participant Controller
       participant Service
       participant Repository
       participant Database

       Client->>Controller: POST /v1/endpoint
       Controller->>Service: process(request)
       Service->>Repository: findByXxx()
       Repository->>Database: SELECT query
       Database-->>Repository: Result
       Repository-->>Service: Entity
       Service-->>Controller: Response
       Controller-->>Client: HTTP 200
   ```
   ````

   **Architecture Diagram** — illustrate relationships between this service and other services/infrastructure:

   ````markdown
   ```mermaid
   graph TB
       subgraph "Service Name"
           A[Controller Layer]
           B[Service Layer]
           C[Repository Layer]
       end
       D[(Database)]
       E[(Redis Cache)]
       F[External API]
       G[Other Service]

       A --> B
       B --> C
       C --> D
       B --> E
       B --> F
       G --> A
   ```
   ````

   **Note**: If the structure is too complex for Mermaid, use ASCII Art as a fallback. But always try Mermaid first.

## 5. Show Final Status

After completing all steps, display a summary report.

---

**Output On Success**

```
## Base Knowledge Initialized

**Source**: Copied from git framework
**Customized**: requirement_standards.md [yes/no]

**Directories:**
- `base_knowledge/common_rules/` (from framework)
- `base_knowledge/standards/` (from framework + reasoned standards)
- `base_knowledge/structures/` (from framework + system_overview.md)

**Standards derived (from requirement_standards.md):**
- coding_standard.md (Base) ✓
- logging_standard.md (Base) ✓
- error_handling_standard.md (Base) ✓
- [extend standards if applicable]

**Structures mapped:**
- system_overview.md (tech stack + architecture overview)

Base Knowledge system is ready!
Review the files in `base_knowledge/` and adjust as needed.
To document individual services: `/initial-project --structures`
Or for a specific service: `/initial-project --service <name>`
```

---

**Artifact Creation Guidelines**

- Standards files must cite **actual class names and file paths** from the codebase.
- Standards must reflect **dominant patterns** — patterns used most frequently and consistently.
- Structure files must name **real classes/files** in the codebase so developers can use them as reference points.
- Mermaid code blocks must use the standard ` ```mermaid ` fence so Markdown viewers can render them.
- Use **Explore Mode thinking**: be curious, investigate deeply, surface hidden complexity.
- Each file must be **self-contained** — readable on its own without requiring other files.
- `system_overview.md` tech stack MUST be extracted from real config files — no guessing.

**Guardrails**

- **MUST HALT** immediately if `base_knowledge/` does not exist at project root — do NOT create it.
- **DO NOT** ask user any questions about base_knowledge setup — user handles prerequisites manually.
- **DO NOT** copy base_knowledge from framework — user does this manually before running.
- **DO NOT** overwrite existing standards or structure files without asking the user first.
- **DO** pick the most frequent and consistent patterns when deriving standards from code.
- **DO** ensure Mermaid code blocks are properly fenced with ` ```mermaid ` for correct rendering.
- **DO** reference actual class names and file paths in structure files — no placeholder names.
- **DO NOT** include sensitive data (passwords, secrets, tokens) in any documentation.
- **DO** read `requirement_standards.md` before starting the Explore Standards step — this file drives the reasoning process.
- **DO** create `system_overview.md` BEFORE individual service files in the Structures step.
- If the project is too large to fully analyze in one session, suggest creating a Change Request via `/opsx:propose init-base-standards` for incremental work.
