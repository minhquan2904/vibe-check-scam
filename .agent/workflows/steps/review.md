---
description: Post-implementation review — generate tracking artifacts (todo-uncover, new-apis, delta-spec)
---

Review implemented code and generate tracking artifacts documenting TODOs, new APIs, and behavioral deltas.

**Pipeline position**: `/mf-init` → `/mf-srs` → `/mf-specs` → `/mf-design` → `/mf-tasks` → `/mf-apply` → **`/mf-review`** → `/mf-archive`

**Input**: Change name (kebab-case). Source code must be implemented (from `/mf-apply`).

**Steps**

1. **Invoke mf-review skill**
   Read and rigorously follow the instructions in `.agent/skills/mf-review/SKILL.md`.

This skill performs:
- Scan implemented source code for TODOs/FIXMEs → `todo-uncover.md`
- Document new API endpoints (path, method, request, response, purpose) → `new-apis.md`
- Compare behavior before/after → `delta-spec.md`
- **If CR variant**: also generate `regression-check.md` and `migration-note.md`

**Output** (Feature variant):
- `openspec/changes/<name>/todo-uncover.md`
- `openspec/changes/<name>/new-apis.md`
- `openspec/changes/<name>/delta-spec.md`

**Output** (CR variant — additional):
- `openspec/changes/<name>/regression-check.md`
- `openspec/changes/<name>/migration-note.md`

When ready to proceed, run `/mf-archive <name>`
