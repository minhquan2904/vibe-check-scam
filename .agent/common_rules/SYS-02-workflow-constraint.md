---
description: Workflow Constraint — Plan→Act→Reflect loop, Fail Fast, Approval Gate, Context Efficiency
---

> 🔴 **TL;DR:** Always follow Plan → Act → Reflect. Tool errors → retry max 2 times then Fail Fast (NO dummy data). Structural changes (DB/API) require Approval Gate. Handoff passes only final output, never raw logs.

# Workflow Constraints

## 1. Reasoning Loop (Plan → Act → Reflect)
1. **PLAN:** Declare approach, intended tools, expected output.
2. **ACT:** Execute using specific Skills/Tools.
3. **REFLECT:** Evaluate results against plan. Restart if incorrect.

## 2. Fail Fast & Recovery
- Tool returns error → **MAX 2 RETRIES** with adjusted parameters.
- After 2 failures → **FAIL FAST**: stop immediately, report error to user with an open question for guidance.
- **NEVER** substitute dummy data to continue the workflow.

## 3. Approval Gate (Human-In-The-Loop)
- Structural changes (DB scripts, API interfaces) → MUST present plan and wait for explicit user approval.
- **FORBIDDEN** to auto-execute shell commands that modify Prod/Staging environments.

## 4. Context Efficiency
- When handing off between steps, pass ONLY final Output/Findings.
- **NEVER** pass raw logs, full page text, or unprocessed data to the next step.
