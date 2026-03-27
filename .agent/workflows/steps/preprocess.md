---
description: Pre-process URD into structured specification ready for OpenSpec ingestion
---

**[WORKFLOW]** Orchestrate the URD pre-processing pipeline → `pre_openspec.md`.

- **Primary skill**: `wf-pre-openspec` — handles all analysis logic, FR normalization, quality scoring
- **Support skill**: `confluence-reader` — reads content from Confluence (page URL / ID / tree) when input is a Confluence source

**Pipeline position**: **`/wf_pre_openspec`** → `/wf_openspec` → `/wf_openspec_apply` → `/opsx-archive`

**Input**: Argument after the command is the URD source (Confluence URL, page ID, file path, or raw text).

**Steps**

1. **If input is a Confluence URL or Page ID**:
   Invoke the `confluence-reader` skill to read the page content (including sub-pages if any) before processing.
   Read and rigorously follow: `.agent/skills/confluence-reader/SKILL.md`

2. **Invoke wf-pre-openspec skill**
   Read and rigorously follow the instructions in `.agent/skills/wf-pre-openspec/SKILL.md`.
   The skill performs:
   - Scaffold OpenSpec change directory via CLI
   - Classify feature type (NEWBUILD or MAINTENANCE) based on `features.md`
   - Analyze URD → normalize FR-xxx + quality scoring + issue detection
   - Output `pre_openspec.md`

**Workflow Specifics:**
- Load any rules defined in `additional_rules` of this workflow frontmatter to govern the extraction behaviors.

When ready to proceed, run `/wf_openspec <feature-name>`