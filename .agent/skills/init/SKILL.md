---
name: init
description: Initialize a new feature — scaffold change directory, generate metadata.yaml and proposal.md. Use when starting a new feature from user description, Confluence pages, or documentation files.
---

Initialize a new feature change by collecting user input from multiple sources, scaffolding the change directory via OpenSpec CLI, and generating the first two artifacts.

**Output**: `metadata.yaml` + `proposal.md` in `openspec/changes/<name>/`

---

**Input**: The user's request should include a feature name (kebab-case) OR a description of what they want to build. The user may also provide input sources.

| Source Type | How to Identify | How to Collect |
|-------------|----------------|----------------|
| **Text description** | User describes the feature directly | Use as-is |
| **Confluence page** | User mentions Confluence URL or page ID | Read via Confluence MCP server |
| **Documentation files** | Paths to markdown/doc files | Read directly from file system |
| **Codebase references** | User specifies modules, services, or file paths | Scan the specified source code |

**Steps**

1. **Gather feature information and classify inputs**

   If the user has not provided clear input, ask:
   > "What feature do you want to build? Please describe it and provide any relevant sources:
   > - Business requirements document (Confluence page ID or link)
   > - Related documentation files (paths or URLs)
   > - Existing code modules to reference (package/service names or paths)"

   From the description, derive a kebab-case name (e.g., "add payment notification" → `add-payment-notification`).

   **IMPORTANT**: Do NOT proceed without understanding what the user wants to build.

2. **Collect context from input sources**

   Process each input source:

   a. **Confluence pages**: Use MCP server to retrieve page content. Extract requirements, acceptance criteria.
   b. **Documentation files**: Read markdown files. Extract standards, structures, rules.
   c. **Codebase references**: Scan specified modules. Identify patterns, interfaces, integration points.

   Compile a **Context Summary** listing all sources read and key information extracted.

3. **Scaffold change directory**

   ```bash
   openspec new change "<name>"
   ```

   This creates `openspec/changes/<name>/` with `.openspec.yaml`.

4. **Generate `metadata.yaml`**

   Create `openspec/changes/<name>/metadata.yaml`:

   ```yaml
   id: "<PROJECT_ID>"  # Format: UPPERCASE(rootProject.name) + YYMMDD + 5 random alphanumeric
   name: "<change-name>"
   type: "new-feature"  # or "change-request" for CR variant
   created: "YYYY-MM-DDTHH:mm:ss+07:00"
   summary: "<Tóm tắt yêu cầu bằng tiếng Việt>"
   service:
     - "<service-1>"
   path:
     - "<endpoint-1>"
   confluence:
     - id: "<page-id>"
       name: "<page-title>"
   jira:
     - id: "<ticket-id>"
       name: "<ticket-title>"
   ```

   **Rules:**
   - `id`: `UPPERCASE(projectName)` + `YYMMDD` + 5 random alphanumeric. Read project name from `.sln` or `.csproj` file at project root.
   - `created`: exact current timestamp with timezone
   - `summary`: Vietnamese
   - `confluence`/`jira`: from user input, empty `[]` if not provided

5. **Load context for proposal**

   Read `openspec/mapping/artifact_context_modular.yml` → section `proposal`:
   - Resolve `context` paths → read knowledge files
   - Resolve `rules` paths → read constraint files (if any)

6. **Determine Feature Profile**

   Cross-reference loaded knowledge to determine:

   a. **Mode**:
      - CRUD (standard module) or REPORT (report module)
      - Lock: code generation pattern

   b. **Entity base class**:
      - `BaseFieldEntity` (standard) / `BaseFieldApprovableEntity` (approval workflow) / `BaseBoEntity` (junction)
      - Lock: entity inheritance

   c. **Service pattern**:
      - Custom logic (`IScoped`) or Report (`BaseCrossReportService`)
      - Lock: service base class

   d. **Feature type**:
      - NEWBUILD vs MAINTENANCE
      - Check against `features.md` if available

   Show locked profile:
   ```
   Feature Profile:
   - Mode: CRUD/REPORT
   - Entity Base: BaseFieldEntity/BaseFieldApprovableEntity/BaseBoEntity
   - Service: IScoped / BaseCrossReportService
   - Controller: BaseController
   - Type: NEWBUILD/MAINTENANCE
   ```

7. **Generate `proposal.md`**

   ```bash
   openspec instructions proposal --change "<name>" --json
   ```

   Parse the JSON response:
   - `template`: structural blueprint for the output
   - `instruction`: guidance for this artifact type
   - `context`/`rules`: constraints for the agent (DO NOT copy into output)
   - `outputPath`: where to write

   Create `proposal.md` following the template structure. Fill sections using:
   - Collected input context from Step 2
   - Feature Profile from Step 6
   - Knowledge from Step 5

8. **Show final status**

   ```bash
   openspec status --change "<name>"
   ```

   Summarize: change name, location, Feature Profile, input sources used, artifacts created.
   Prompt: "Run `/mf-srs <name>` to generate SRS."

**Guardrails**
- MUST collect input from ALL sources provided before generating any artifact
- MUST generate `metadata.yaml` BEFORE `proposal.md`
- `context` and `rules` from openspec instructions are constraints — DO NOT copy into output files
- If change already exists → ask if user wants to continue or recreate
- Verify artifact files exist after writing
