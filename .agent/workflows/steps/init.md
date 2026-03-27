---
description: Initialize a new feature — scaffold change directory, generate metadata and proposal
---

Initialize a new feature change by collecting user input, scaffolding the change directory, and generating the first two artifacts.

**Pipeline position**: **`/mf-init`** → `/mf-srs` → `/mf-specs` → `/mf-design` → `/mf-tasks` → `/mf-apply` → `/mf-review` → `/mf-archive`

**Input**: Feature name (kebab-case) + description. User may also provide input sources (Confluence page, doc files, code modules).

**Steps**

1. **Invoke mf-init skill**
   Read and rigorously follow the instructions in `.agent/skills/mf-init/SKILL.md`.

This skill performs:
- Collect user input (text description, Confluence pages via MCP, documentation files, codebase references)
- `openspec new change "<name>"` — scaffold change directory
- Generate `metadata.yaml` — change tracking (ID, services, endpoints, timestamps)
- Load context from `artifact_context_modular.yml` → section `proposal`
- Determine Feature Profile (flow type, factory type, NEWBUILD/MAINTENANCE)
- `openspec instructions proposal --change "<name>" --json` — get template + instructions
- Generate `proposal.md`

**Output**: `openspec/changes/<name>/metadata.yaml` + `openspec/changes/<name>/proposal.md`

When ready to proceed, run `/mf-srs <name>`
