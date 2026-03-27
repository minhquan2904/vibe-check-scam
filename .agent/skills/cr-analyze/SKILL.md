---
name: cr-analyze
description: Analyze existing codebase for a Change Request — scan all layers, generate current-code-logic, compare-logic, and proposal.
---

Deep-analyze the existing codebase for a Change Request. Generate a snapshot of current code logic, a code-vs-documentation comparison, and a CR proposal.

**Output**: `metadata.yaml` + `current-code-logic.md` + `compare-logic.md` + `proposal.md`

---

**Input**: CR name (kebab-case) + description of the change. User may specify affected modules/services.

**Steps**

1. **Gather CR information**

   If not provided, ask:
   > "What change do you want to make? Please describe:
   > - What needs to change
   > - Which modules/services are affected
   > - Any Confluence/Jira references"

   Derive kebab-case name from description.

2. **Scaffold change directory**

   ```bash
   openspec new change "<name>"
   ```

3. **Scan existing codebase**

   Perform deep analysis of affected modules. Scan all layers:

   a. **Controller layer**: REST endpoints, `[Authorize]`, `BaseController`, thin controller
   b. **Service layer**: Business logic, `IScoped`, DI trực tiếp
   c. **Entity layer**: `BaseFieldEntity` / `BaseFieldApprovableEntity`, `[Table]`/`[Column]`
   d. **DTO layer**: Request/Response objects, `FilterRequest`, `BaseResponse`
   e. **DbContext**: Entity registration, Oracle connection
   f. **Infrastructure**: FunctionConst, enums, error codes, config

4. **Generate `current-code-logic.md`**

   Detailed snapshot of the current system:

   ```markdown
   # Current Code Logic: <module-name>

   ## Processing Flow
   [Step-by-step flow through the code layers — class by class]

   ## Configuration Keys
   | Key | Value/Default | Source | Used By |
   |-----|--------------|--------|---------|

   ## Message Codes
   | Code | Constant | Message | Used In |
   |------|----------|---------|---------|

   ## Error Codes
   | Code | Constant | Condition | Service |
   |------|----------|-----------|---------|

   ## External Integrations
   | System | Endpoint/Method | Purpose | Called From |
   |--------|----------------|---------|------------|

   ## Key Classes
   | Class | Layer | Responsibility | Base Class |
   |-------|-------|---------------|------------|
   ```

5. **Generate `compare-logic.md`**

   Map code ↔ documentation:

   ```markdown
   # Code vs Documentation Comparison

   ## Status Legend
   - [V] Matched — code and documentation agree
   - [!] Divergent — code differs from documentation
   - [X] Missing — exists in code but not in documentation
   - [+] Doc Only — exists in documentation but not in code

   ## Comparison Table
   | # | Logic Point | Code State | Doc State | Status | Gap Analysis |
   |---|------------|-----------|-----------|--------|-------------|
   ```

   Gap Analysis values: No Change / Modify / New / Remove

6. **Generate `metadata.yaml`**

   Same format as `/mf-init` but with `type: "change-request"`.

7. **Generate `proposal.md`**

   Load context from `openspec/mapping/artifact_context_modular.yml` → section `proposal`.

   ```bash
   openspec instructions proposal --change "<name>" --json
   ```

   Create CR-specific proposal including:
   - Why this CR is needed (referencing current-code-logic findings)
   - Impact analysis (from code scan)
   - Risk assessment
   - Scope of changes

8. **Show summary**

   Display: current-code-logic highlights, compare-logic status counts, Feature Profile.
   Prompt: "Run `/mf-srs <name>` to generate SRS."

**Guardrails**
- MUST analyze ACTUAL codebase — do NOT guess
- MUST create ALL 4 output artifacts
- MUST generate `current-code-logic.md` BEFORE `compare-logic.md` BEFORE `proposal.md`
- MUST map code ↔ doc — if no documentation found, state explicitly
- DO NOT implement code — only analyze and document
