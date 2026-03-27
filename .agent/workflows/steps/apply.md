---
description: Implement tasks — generate source code based on tasks and design with project standards enforcement
---

Implement all tasks from tasks.md, generating actual source code with mandatory project standards compliance.

**Pipeline position**: `/mf-init` → `/mf-srs` → `/mf-specs` → `/mf-design` → `/mf-tasks` → **`/mf-apply`** → `/mf-review` → `/mf-archive`

**Input**: Change name (kebab-case). Change directory must contain `tasks.md` and `design.md`.

**Steps**

1. **Invoke mf-apply skill**
   Read and rigorously follow the instructions in `.agent/skills/mf-apply/SKILL.md`.

This skill performs:
- Load apply knowledge from `artifact_context_modular.yml` → section `apply`
- Read all change artifacts: `proposal.md`, `srs.md`, `design.md`, `tasks.md`, `specs/`
- **If CR variant**: also read `current-code-logic.md` and `compare-logic.md`
- Determine Feature Profile (flow type, handler class, factory class)
- Read and comply with project standards (coding, logging, error handling)
- Loop through tasks: implement each task, cross-check against Feature Profile
- `openspec instructions apply --change "<name>" --json` — get instructions

**Output**: Source code files at project location

When ready to proceed, run `/mf-review <name>`
