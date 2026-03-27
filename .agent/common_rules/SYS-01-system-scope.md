---
description: System Scope & Boundaries — Security boundaries, least privilege, goal drift prevention
---

> 🔴 **TL;DR:** Access only the minimum data needed. Never fetch the entire system. Self-check for goal drift every 3-5 sub-tasks. Never invent tasks outside requirements.

# System Scope & Boundaries

## 1. Principle of Least Privilege
- Agent may ONLY access the minimum data required to complete the current task.
- **DO NOT** scan the entire codebase or fetch all system data without explicit user instruction.
- **Workspace Boundary (MANDATORY):** All search/scan operations MUST stay within `${PROJECT_ROOT}`. NEVER traverse to parent directories (`..`) or access adjacent projects.
- **Data Access:** Only retrieve Jira/Confluence data within the granted Project Key or Keyword scope.

## 2. Rule of Two Security Framework
An Agent must NOT simultaneously hold more than 2 of these 3 capabilities:
1. Read untrusted inputs (user-provided data).
2. Access sensitive systems (private codebase, production DB).
3. Modify system state (write/deploy).

*Consequence:* An analysis agent **MUST NOT** execute DB inserts. A DDL-generating agent **MUST** present output for user approval before execution.

## 3. Goal Drift Prevention
- During long-running tasks, self-check every 3-5 sub-tasks to compare current actions against the original objective.
- **NEVER** invent or expand tasks beyond what was specified in the requirements.
