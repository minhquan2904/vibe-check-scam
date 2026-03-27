---
name: specs
description: Generate delta behavioral specs (ADDED/MODIFIED/REMOVED) with scenarios for each requirement.
---

Generate behavioral specifications using delta format based on proposal and SRS.

**Output**: `specs/<domain>/spec.md` in `openspec/changes/<name>/`

---

**Input**: Change name (kebab-case). `proposal.md` and `srs.md` must exist.

**Steps**

1. **Verify prerequisites**

   Check that `proposal.md` and `srs.md` exist in change directory.

2. **Read dependency artifacts**

   - `proposal.md` → scope, services, requirements
   - `srs.md` → detailed requirements, API specs, flow type

3. **⛔ MANDATORY COMPLIANCE CHECK (HALT if failed)**

   Read `openspec/mapping/artifact_context_modular.yml` → section `specs`.

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
   Resolve `context` glob paths → read knowledge files. WARN if empty, do not HALT.

   > **CRITICAL**: MUST NOT proceed to Step 4 until ALL `required_rules` and `required_skills` are loaded. No exceptions.

4. **Get specs instructions**

   ```bash
   openspec instructions specs --change "<name>" --json
   ```

5. **Generate spec files**

   For each affected domain/module:
   - Create `specs/<domain>/spec.md` following template
   - Write ONLY delta: ADDED / MODIFIED / REMOVED
   - Each requirement MUST have at least 1 Scenario (GIVEN/WHEN/THEN)
   - Use RFC 2119 keywords: MUST, SHALL, SHOULD, MAY
   - Map each requirement to source (proposal/srs)

   Apply Spec Rules from `PRJ-rule_planing_feature.md` as additional constraints.

6. **Cross-check**

   - ✓ All requirements from proposal/srs are covered
   - ✓ No duplicate requirements
   - ✓ No implementation details in specs
   - ✓ Every requirement has at least 1 scenario

7. **Show status**

   Prompt: "Run `/mf-design <name>` to generate technical design."

**Guardrails**
- MUST read proposal + srs before generating
- MUST apply Spec Rules from `PRJ-rule_planing_feature.md`
- MUST NOT include implementation details
- MUST NOT lose any requirement from proposal/srs
