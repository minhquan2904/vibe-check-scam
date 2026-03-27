---
description: Coding Discipline — Root-Cause Tracing, Blast Radius Analysis, Read-Before-Write (DRY), No Hallucination
---

> 🔴 **TL;DR:** Never fix immediately — analyze Root Cause first. Assess Blast Radius before modifying. Search for existing patterns before writing new code. Only use facts — never fabricate APIs/methods.

# Coding Discipline

## 1. Root-Cause Tracing (Debugging)
When receiving error logs or bug reports, **DO NOT propose a fix immediately**:

1. **Analyze Context:** Error message, stack trace, related files.
2. **5 Whys Technique:** Ask "Why?" repeatedly to drill from surface symptoms to root logic.
3. **Present Hypothesis:** "I suspect the root cause is [X] because [Y]."
4. **Verify First:** Propose a log statement, debug command, or specific value check to prove the hypothesis before writing any fix code.

## 2. Blast Radius Analysis
**BEFORE** modifying any method, class, interface, or DB schema:

1. **Find Dependencies:** Search for ALL files that call or depend on the component being changed.
2. **List Risks:** Enumerate specific side-effects that could occur in other modules.
3. **Propose Sync Plan:** Suggest accompanying changes to prevent system breakage.

## 3. Read-Before-Write (DRY)
To maintain project consistency and the DRY principle:

1. **Don't create blindly:** Before writing a new Helper, Service, or Component → **MUST** search for existing similar components first.
2. **Inherit Architecture:** Analyze 1-2 similar files to replicate coding style, error handling, and directory structure.
3. **Report findings:** Begin with: "I found a similar pattern in [file X]. I will apply this pattern."

## 4. No Hallucination (Explicit Uncertainty)

1. **Facts only:** Code must be based on official documentation or existing workspace code. **NEVER** fabricate APIs, properties, or methods.
2. **Stop & Ask:** If not 100% certain about a library OR if an ENV variable is missing → **STOP** and ask the user.
3. **Uncertainty syntax:** "I need more information about [library X] version [Y] to complete this task."
