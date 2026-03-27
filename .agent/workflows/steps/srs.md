---
description: Generate SRS — consolidate all inputs into a Software Requirements Specification
---

Generate the Software Requirements Specification by consolidating proposal and all collected input sources.

**Pipeline position**: `/mf-init` → **`/mf-srs`** → `/mf-specs` → `/mf-design` → `/mf-tasks` → `/mf-apply` → `/mf-review` → `/mf-archive`

**Input**: Change name (kebab-case). Change directory must contain `proposal.md` (from `/mf-init`).

**Steps**

1. **Invoke mf-srs skill**
   Read and rigorously follow the instructions in `.agent/skills/mf-srs/SKILL.md`.

This skill performs:
- Read `proposal.md` from change directory
- Collect additional context from input sources (Confluence via MCP, documentation files, codebase scan)
- Load context from `artifact_context_modular.yml` → section `srs`
- Load Planning Rules from `base_knowledge/common_rules/PRJ-rule_planing_feature.md`
- `openspec instructions srs --change "<name>" --json` — get template + instructions
- Generate `srs.md`

**Output**: `openspec/changes/<name>/srs.md`

When ready to proceed, run `/mf-specs <name>`
