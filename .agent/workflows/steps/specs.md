---
description: Generate behavioral specs — delta format with ADDED/MODIFIED/REMOVED requirements
---

Generate behavioral specifications using delta format based on proposal and SRS.

**Pipeline position**: `/mf-init` → `/mf-srs` → **`/mf-specs`** → `/mf-design` → `/mf-tasks` → `/mf-apply` → `/mf-review` → `/mf-archive`

**Input**: Change name (kebab-case). Change directory must contain `proposal.md` and `srs.md`.

**Steps**

1. **Invoke mf-specs skill**
   Read and rigorously follow the instructions in `.agent/skills/mf-specs/SKILL.md`.

This skill performs:
- Read `proposal.md` and `srs.md` from change directory
- Load context from `artifact_context_modular.yml` → section `specs`
- Load Spec Rules from `base_knowledge/common_rules/PRJ-rule_planing_feature.md`
- `openspec instructions specs --change "<name>" --json` — get template + instructions
- Generate `specs/<domain>/spec.md` for each affected domain
- Cross-check: all requirements covered, RFC 2119 keywords used

**Output**: `openspec/changes/<name>/specs/**/*.md`

When ready to proceed, run `/mf-design <name>`
