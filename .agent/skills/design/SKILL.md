---
name: design
description: Generate technical design — architecture mapping, API contracts, data model, component structure.
---

Generate the technical design document based on proposal, SRS, and behavioral specs.

**Output**: `design.md` in `openspec/changes/<name>/`

---

**Input**: Change name (kebab-case). `proposal.md`, `srs.md`, and `specs/` must exist.

**Steps**

1. **Verify prerequisites**

   Check that `proposal.md`, `srs.md`, and at least one file in `specs/` exist.

2. **Read dependency artifacts**

   - `proposal.md` → Feature Profile, affected services
   - `srs.md` → API specs, flow phases, error codes
   - `specs/**/*.md` → behavioral requirements

3. **⛔ MANDATORY COMPLIANCE CHECK (HALT if failed)**

   Read `openspec/mapping/artifact_context_modular.yml` → section `design`.

   **Step 3a — Load `required_rules` (HALT if missing):**
   For EACH file listed in `required_rules`:
   - Check file exists → if NOT → **HALT IMMEDIATELY**:
     ```
     ❌ HALT: Required rule file not found: <path>
     Cannot proceed without this rule. Please create it or remove from required_rules.
     ```
   - If exists → **READ the entire file into memory**
   - Display: `✅ Rule loaded: <path>`

   **Step 3b — Load `required_skills` (HALT if missing):**
   Same process for `required_skills`.

   **Step 3c — Load `context` (soft — warn if missing):**
   Resolve `context` glob paths → read knowledge files (architecture, entity, service, controller, DTO patterns). WARN if empty, do not HALT.

   > **CRITICAL**: MUST NOT proceed to Step 4 until ALL `required_rules` and `required_skills` are loaded. No exceptions.

4. **Verify Feature Profile**

   Re-determine and lock:
   - Mode → CRUD or REPORT
   - Entity base class → `BaseFieldEntity` / `BaseFieldApprovableEntity` / `BaseBoEntity`
   - Service pattern → `IScoped` or `BaseCrossReportService`
   - Feature type → NEWBUILD/MAINTENANCE

   Show profile before proceeding.

5. **Get design instructions**

   ```bash
   openspec instructions design --change "<name>" --json
   ```

6. **Generate `design.md`**

   Follow template structure. Include ALL sections:
   - Architecture Overview (mermaid diagram)
   - Component Mapping (entity base + service pattern + controller)
   - Entity Design (Oracle table schema, `[Table]`/`[Column]` attributes)
   - API Design (per endpoint: request/response/validation/authorization)
   - Class Specifications (Entity, DTOs, Service, Controller, FunctionConst)
   - Package Structure (BOBase.Domain + BOModule.{Feature})
   - Frontend Design (Angular module, components, services)
   - Cross-Module Dependencies

   Apply Design Rules from `PRJ-rule_planing_feature.md`.

7. **Cross-check**

   - ✓ Base classes match locked Feature Profile
   - ✓ All layers covered (Entity → DTO → Service → Controller + Angular)
   - ✓ All specs mapped to design components
   - ✓ Template structure preserved

8. **Show status**

   Prompt: "Run `/mf-tasks <name>` to generate implementation tasks."

**Guardrails**
- MUST verify Feature Profile before generating
- MUST apply Design Rules from `PRJ-rule_planing_feature.md`
- MUST NOT skip any layer in the architecture
- MUST NOT invent logic outside specs
- Cross-check every section against Feature Profile
