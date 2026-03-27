---
name: archive
description: Archive a completed change with code-vs-docs sync check, optional changelog, and knowledge export advisory.
---

Archive a completed feature or change request after verifying documentation is in sync with code.

**Output**: Synced documentation + archive move. CR variant: `changelog.md` + updated `compare-logic.md`.

---

**Input**: Change name (kebab-case). All steps (init → review) must be complete.

**Steps**

1. **Verify completion**

   ```bash
   openspec status --change "<name>"
   ```

   Check all artifacts and implementation are done. If incomplete → warn user.

2. **Detect variant**

   Check `metadata.yaml` → `type` to determine feature vs CR.

3. **Code-vs-Documentation Sync Check**

   Compare actual source code changes against documentation artifacts:
   - `proposal.md` — scope matches implementation?
   - `design.md` — classes, APIs, schemas match code?
   - `specs/**/*.md` — behavioral requirements implemented?

   a. **If MATCH**: Display `[OK] Documentation is in sync with source code`

   b. **If MISMATCH**: 
      - Auto-edit documentation to match actual implementation
      - Add `> [!WARNING]` markers in corrected sections
      - Display detailed mismatch report:
        ```
        Mismatch Report:
        | Document | Section | Expected | Actual | Action |
        |----------|---------|----------|--------|--------|
        ```

   **Rule**: MUST only edit documentation to match code — NEVER edit code to match documentation.

4. **If CR variant: Update `compare-logic.md`**

   Update the comparison table with post-implementation state:
   - Show Before → After for each logic point
   - Mark status: [V] Matched / [!] Divergent / [X] Removed / [+] Added

5. **If CR variant: Generate `changelog.md`**

   ```markdown
   # Changelog: <change-name>

   ## Summary
   <one-line summary>

   ## Changes
   ### Added
   - [description]

   ### Modified
   - [description]

   ### Removed
   - [description]

   ### Fixed
   - [description]

   ## Migration Steps
   1. [step]

   ## Risk Notes
   - [risk]
   ```

6. **Archive Move**

   Move change directory:
   ```
   openspec/changes/<name>/  →  openspec/archive/<name>/
   ```

   Verify all files are included:
   - Core artifacts: metadata.yaml, proposal.md, srs.md, specs/, design.md, tasks.md
   - Tracking: todo-uncover.md, new-apis.md, delta-spec.md
   - CR-specific (if applicable): current-code-logic.md, compare-logic.md, regression-check.md, migration-note.md, changelog.md

7. **Knowledge Export Advisory**

   Check if the change might affect `base_knowledge/`:
   - New patterns not in existing knowledge?
   - Changes to architecture, factory patterns, handler patterns?

   If potentially affected:
   > ⚠️ This change may affect base_knowledge. Consider running `/mf-learn` or manually updating knowledge files.

   **DO NOT auto-update `base_knowledge/`** — only advise.

**Guardrails**
- MUST perform code-vs-docs sync BEFORE archiving
- MUST only edit docs to match code — NEVER reverse
- MUST add `> [!WARNING]` markers when auto-correcting
- MUST NOT auto-update `base_knowledge/` — only warn
- MUST verify all files are included in archive
