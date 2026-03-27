---
description: Tool Usage Constraint — Precise parameters, token efficiency, judicious tool selection
---

> 🔴 **TL;DR:** Tool parameters must be exact — NEVER guess. Use pagination/filtering (`limit=5~10`). Read files by block. Tool errors → log + retry, 2 failures → Fail Fast. Don't use scripts when LLM reasoning suffices.

# Tool Usage Constraints

## 1. Precise Parameters
- Analyze user input to extract exact tool parameters (Key, URL, ID, Keywords). Guessing parameters is **FORBIDDEN**.
- If parameters are insufficient → **STOP** and ask user to provide missing information.

## 2. Token Efficiency
- Use **pagination/filtering**: `limit=5` or `limit=10`. Never fetch all data at once.
- Read files by block (`StartLine`, `EndLine`), not the entire file when unnecessary.

## 3. Judicious Tool Selection
- **DO NOT** use Python scripts to parse files when `view_file` + LLM reasoning is sufficient.
- Use shell commands ONLY when OS interaction is truly needed (compile, test, git).

## 4. Respect Tool Errors
- Tool throws error → log it, retry with adjusted parameters.
- After 2 failures → Fail Fast (see SYS-02).
