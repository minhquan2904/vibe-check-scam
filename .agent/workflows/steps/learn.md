---
description: Learn codebase — scan and generate knowledge files for propose and apply phases
---

Learn the project codebase by scanning architecture, patterns, and practices, then generating knowledge files used as context throughout the pipeline.

**When to use**: Run once after initial project setup, or when codebase has significant architectural changes.

**Input**: No arguments required. Scans the entire project codebase.

**Steps**

1. **Invoke mf-learn skill**
   Read and rigorously follow the instructions in `.agent/skills/mf-learn/SKILL.md`.

This skill performs 9 sequential learn steps:

**Propose Knowledge** (ghi vào `base_knowledge/structures/propose/`):
1. Learn Architecture → `knowledge_architecture.md`
2. Learn Factory Patterns → `knowledge_factory.md`
3. Learn Transaction Flows → `knowledge_transaction_flow.md`
4. Learn Third-Party Calls → `knowledge_thirdparty_call.md`
5. Learn Handlers → `knowledge_handler.md`
6. Learn DTO/Entity Patterns → `knowledge_dto_entity.md`
7. Learn Common Utils → `knowledge_common_utils.md`

**Apply Knowledge** (ghi vào `base_knowledge/structures/apply/`):
8. Learn Code Practices → `knowledge_code_practices.md`

**Features Registry**:
9. Scan all modules → `base_knowledge/structures/features.md`

**Output**: 8 knowledge files + 1 features registry

> **Note**: If the codebase is large, each learn step may require its own conversation. Use `/mf-learn --step <N>` to run individual steps.
