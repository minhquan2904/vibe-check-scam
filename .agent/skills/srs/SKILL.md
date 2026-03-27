---
name: srs
description: Generate Software Requirements Specification by consolidating proposal and multi-source inputs into a structured SRS document.
---

Generate the SRS document by reading the proposal, collecting additional input context, and producing a consolidated specification.

**Output**: `srs.md` in `openspec/changes/<name>/`

---

**Input**: Change name (kebab-case). `proposal.md` must exist in the change directory.

**Steps**

1. **Verify prerequisites**

   Check that `openspec/changes/<name>/proposal.md` exists.
   If not → notify user to run `/mf-init` first.

2. **Read proposal**

   Read `openspec/changes/<name>/proposal.md` to understand:
   - Feature scope and business flow
   - Flow type and affected services
   - Feature Profile

3. **Collect additional context** (if needed)

   If the user provides additional input sources in this conversation:
   - Confluence pages → read via MCP
   - Documentation files → read from file system
   - Codebase references → scan source code

   If no additional sources → use information from proposal only.

4. **⛔ MANDATORY COMPLIANCE CHECK (HALT if failed)**

   Read `openspec/mapping/artifact_context_modular.yml` → section `srs`.

   **Step 4a — Load `required_rules` (HALT if missing):**
   For EACH file listed in `required_rules`:
   - Check file exists → if NOT → **HALT IMMEDIATELY**:
     ```
     ❌ HALT: Required rule file not found: <path>
     Cannot proceed without this rule. Please create it or remove from required_rules.
     ```
   - If exists → **READ the entire file into memory**
   - Display confirmation:
     ```
     ✅ Rule loaded: <path>
     ```

   **Step 4b — Load `required_skills` (HALT if missing):**
   Same process for `required_skills`.

   **Step 4c — Load `context` (soft — warn if missing):**
   Resolve `context` glob paths → read knowledge files. WARN if directory empty but do not HALT.

   > **CRITICAL**: You MUST NOT proceed to Step 5 until ALL `required_rules` and `required_skills` are loaded and confirmed. No exceptions.

5. **Get SRS instructions**

   ```bash
   openspec instructions srs --change "<name>" --json
   ```

   Parse: `template`, `instruction`, `context`, `rules`, `outputPath`.

6. **Generate `srs.md`**

   Follow template structure from Step 5. Fill using:
   - Proposal content (Step 2)
   - Additional collected context (Step 3)
   - Knowledge and rules (Step 4)

   **SRS must include** (based on flow type):
   - Command/Query: Phase 1 only
   - Financial: Phase 1 + 2 + 3, with Revert Chain
   - NonFinancial: Phase 1 + 3
   - MAINTENANCE: Change Delta section

   Apply `context` and `rules` as constraints — DO NOT copy into output.

7. **Show status**

   Prompt: "Run `/mf-specs <name>` to generate behavioral specs."

**Guardrails**
- MUST read `proposal.md` before generating
- MUST apply Planning Rules from `PRJ-rule_planing_feature.md`
- SRS must be detailed enough to serve as sole input for design and spec creation
- `context`/`rules` from openspec instructions are constraints — DO NOT include in output
