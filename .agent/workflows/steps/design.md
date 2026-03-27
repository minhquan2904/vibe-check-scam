---
description: Generate technical design — architecture, API contracts, data model, component structure
---

Generate the technical design document based on proposal, SRS, and behavioral specs.

**Pipeline position**: `/mf-init` → `/mf-srs` → `/mf-specs` → **`/mf-design`** → `/mf-tasks` → `/mf-apply` → `/mf-review` → `/mf-archive`

**Input**: Change name (kebab-case). Change directory must contain `proposal.md`, `srs.md`, and `specs/`.

**Steps**

1. **Invoke mf-design skill**
   Read and rigorously follow the instructions in `.agent/skills/mf-design/SKILL.md`.

This skill performs:
- Read `proposal.md`, `srs.md`, `specs/**/*.md` from change directory
- Load context from `artifact_context_modular.yml` → section `design`
- Load Design Rules from `base_knowledge/common_rules/PRJ-rule_planing_feature.md`
- Determine/verify Feature Profile (flow type, factory type, base classes)
- `openspec instructions design --change "<name>" --json` — get template + instructions
- Generate `design.md`
- Cross-check: base classes match profile, all layers covered

**Output**: `openspec/changes/<name>/design.md`

When ready to proceed, run `/mf-tasks <name>`
