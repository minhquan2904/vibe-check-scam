---
name: review
description: Post-implementation review — scan implemented code and generate tracking artifacts (todo-uncover, new-apis, delta-spec).
---

Review implemented source code and generate tracking artifacts documenting discoveries, new APIs, and behavioral deltas.

**Output**: `todo-uncover.md` + `new-apis.md` + `delta-spec.md` (+ CR variant: `regression-check.md` + `migration-note.md`)

---

**Input**: Change name (kebab-case). Implementation must be complete (from `/mf-apply`).

**Steps**

1. **Read change context**

   From `openspec/changes/<name>/`:
   - `design.md` → expected API endpoints, classes
   - `specs/**/*.md` → expected behaviors
   - `tasks.md` → implemented tasks
   - `metadata.yaml` → change type (`new-feature` or `change-request`)

2. **Detect variant**

   Check `metadata.yaml` → `type`:
   - `new-feature` → generate 3 tracking files
   - `change-request` → generate 5 tracking files (3 + regression-check + migration-note)

3. **Scan implemented source code**

   Identify all files created/modified by `/mf-apply` (from tasks.md file paths).

4. **Generate `todo-uncover.md`**

   Scan code for:
   - `TODO` comments
   - `FIXME` comments
   - `HACK` / `WORKAROUND` comments
   - Incomplete implementations (empty methods, placeholder values)
   - Edge cases not covered by specs

   Format:
   ```markdown
   # TODO & Uncover Items

   ## Statistics
   | Type | Count |
   |------|-------|
   | TODO | N |
   | FIXME | N |
   | Edge Cases | N |

   ## Details
   | # | Type | File | Line | Description |
   |---|------|------|------|-------------|
   ```

5. **Generate `new-apis.md`**

   Document all new API endpoints:
   ```markdown
   # New API Endpoints

   | # | Method | Path | Controller | Handler | Description |
   |---|--------|------|------------|---------|-------------|
   ```

   For each endpoint include:
   - Request body structure
   - Response body structure
   - Authentication requirement
   - Purpose/description

6. **Generate `delta-spec.md`**

   Compare before/after behavior:
   ```markdown
   # Delta Specification

   ## Impact Scope
   | Component | Before | After | Impact |
   |-----------|--------|-------|--------|

   ## Behavioral Changes
   | # | Area | Previous | Current | Type |
   |---|------|----------|---------|------|
   ```

7. **If CR variant: Generate `regression-check.md`**

   Read `current-code-logic.md` and verify backward compatibility:
   ```markdown
   # Regression Check

   | # | Existing Behavior | Status | Verification | Notes |
   |---|------------------|--------|-------------|-------|
   | 1 | [behavior] | ✅ Preserved / ⚠️ Modified / ❌ Broken | [how verified] | |
   ```

8. **If CR variant: Generate `migration-note.md`**

   Document deployment requirements:
   ```markdown
   # Migration Notes

   ## Config Changes
   | Key | Old Value | New Value | Required |
   |-----|-----------|-----------|----------|

   ## Database Changes
   | Type | Table/Column | SQL | Rollback |
   |------|-------------|-----|----------|

   ## API Changes (Breaking)
   | Endpoint | Change | Client Impact |
   |----------|--------|---------------|
   ```

9. **Show summary**

   Prompt: "Run `/mf-archive <name>` to archive."

**Guardrails**
- MUST scan actual implemented code, not just specs/design
- MUST detect variant from `metadata.yaml` type field
- MUST generate ALL required tracking files for the variant
