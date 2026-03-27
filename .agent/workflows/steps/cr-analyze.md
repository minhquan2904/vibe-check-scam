---
description: Analyze existing code for Change Request — generate current-code-logic, compare-logic, and proposal
---

Deep-analyze the existing codebase for a Change Request, generating a snapshot of current code logic and a code-vs-documentation comparison before proposing changes.

**Pipeline position**: **`/mf-cr-analyze`** → `/mf-srs` → `/mf-specs` → `/mf-design` → `/mf-tasks` → `/mf-apply` → `/mf-review` → `/mf-archive`

**Input**: CR name (kebab-case) + description of the change. User may specify affected modules/services.

**Steps**

1. **Invoke mf-cr-analyze skill**
   Read and rigorously follow the instructions in `.agent/skills/mf-cr-analyze/SKILL.md`.

This skill performs:
- `openspec new change "<name>"` — scaffold change directory
- Scan all layers: Controller → Service/Handler → Factory → Model → Repository → Infrastructure
- Generate `current-code-logic.md` — detailed snapshot of current processing flow, config keys, message codes, error codes, external integrations
- Map code ↔ documentation → `compare-logic.md` with status tracking ([V] Matched / [!] Divergent / [X] Missing)
- Generate `metadata.yaml` (type: `change-request`)
- Load context from `artifact_context_modular.yml` → section `proposal`
- `openspec instructions proposal --change "<name>" --json` — get template
- Generate `proposal.md` — CR-specific proposal with impact analysis and risk assessment

**Output**:
- `openspec/changes/<name>/metadata.yaml`
- `openspec/changes/<name>/current-code-logic.md`
- `openspec/changes/<name>/compare-logic.md`
- `openspec/changes/<name>/proposal.md`

When ready to proceed, run `/mf-srs <name>`
