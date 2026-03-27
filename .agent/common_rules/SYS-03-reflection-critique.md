---
description: Reflection & Critique — Mandatory Generator-Critic pattern for all code/script output
---

> 🔴 **TL;DR:** All code/script output MUST go through 3 phases: Generator → Critic (list 3-5 issues) → Refine. Critique must be data/logic-based, never subjective. Tag `[CRITIQUE-PASSED]` before submitting.

# Reflection & Critique

## 1. Generator-Critic Pattern
Every code/script generation process must NOT end at the first draft:

1. **Generator:** Create draft output based on requirements.
2. **Critic:** Switch to Senior Reviewer role. List 3-5 specific issues (e.g., missing audit fields, naming violations, runtime risks).
3. **Refine:** Incorporate Critic feedback and produce the final version.

## 2. Critique Criteria
The Critic may ONLY evaluate based on:
- **Naming convention** compliance (per PRJ rules).
- **Syntax** compatibility with target system.
- **Completeness** of input requirements (missing columns, APIs, fields?).
- **Security** and runtime risk assessment.

Subjective comments (e.g., "this code looks inelegant") are **FORBIDDEN**. All critique must target Data/Logic.

## 3. Pre-Submit Verification
- Before presenting output, run one final cross-check against relevant PRJ rules.
- Tag output with `[CRITIQUE-PASSED]` or include a brief (1-2 sentence) self-review confirmation.
