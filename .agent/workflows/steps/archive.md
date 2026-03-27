---
description: Archive a completed change — code-vs-doc sync, changelog generation, and archive move
---

Archive a completed feature or change request with documentation sync verification and optional changelog.

**Pipeline position**: `/mf-init` → `/mf-srs` → `/mf-specs` → `/mf-design` → `/mf-tasks` → `/mf-apply` → `/mf-review` → **`/mf-archive`**

**Input**: Change name (kebab-case). All implementation and review steps must be complete.

**Steps**

1. **Invoke mf-archive skill**
   Read and rigorously follow the instructions in `.agent/skills/mf-archive/SKILL.md`.

This skill performs:

**A. Code-vs-Documentation Sync Check**
- Compare actual source code against change artifacts (proposal, design, specs)
- If MATCH → display "[OK] Documentation is in sync with source code"
- If MISMATCH → auto-edit docs to match code, add `> [!WARNING]` markers

**B. CR-Specific** (if `current-code-logic.md` exists):
- Update `compare-logic.md` with post-implementation state (Before → After)
- Generate `changelog.md` — Added/Modified/Removed/Fixed items with migration steps

**C. Archive Move**
- Move change directory to `openspec/archive/<name>/`
- Verify all tracking artifacts are included

**D. Knowledge Export Advisory**
- Warn if change may affect `base_knowledge/` (DO NOT auto-update)

**Guardrails**
- MUST perform code-vs-docs sync check before archiving
- MUST only edit documentation to match code — never edit code to match documentation
- MUST NOT auto-update `base_knowledge/` — only warn and advise
