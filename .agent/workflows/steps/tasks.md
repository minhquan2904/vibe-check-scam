---
description: Generate implementation tasks — ordered by layer and dependency
---

Generate implementation task breakdown based on specs and design.

**Pipeline position**: `/mf-init` → `/mf-srs` → `/mf-specs` → `/mf-design` → **`/mf-tasks`** → `/mf-apply` → `/mf-review` → `/mf-archive`

**Input**: Change name (kebab-case). Change directory must contain `specs/` and `design.md`.

**Steps**

1. **Invoke mf-tasks skill**
   Read and rigorously follow the instructions in `.agent/skills/mf-tasks/SKILL.md`.

This skill performs:
- Read `specs/**/*.md` and `design.md` from change directory
- Load context from `artifact_context_modular.yml` → section `tasks`
- `openspec instructions tasks --change "<name>" --json` — get template + instructions
- Generate `tasks.md` — task breakdown by layer, dependency order
- Verify traceability: Requirement → Spec → Task (no gaps)

**Output**: `openspec/changes/<name>/tasks.md`

When ready to proceed, run `/mf-apply <name>`
