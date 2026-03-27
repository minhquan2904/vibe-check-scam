---
name: apply
description: Implement tasks from change artifacts using Serena framework's semantic tools, with project standards enforcement and Feature Profile cross-checking.
---

Implement all tasks from `tasks.md`, generating actual source code with mandatory compliance to project standards and Feature Profile.
Uses **Serena MCP tools** for semantic code navigation and editing — NOT raw file reads.

**Output**: Source code files at project location.

---

**Input**: Change name (kebab-case). `tasks.md` and `design.md` must exist.

---

## Serena Tool Reference (used throughout this skill)

| Tool | Category | Purpose |
|------|----------|---------|
| `check_onboarding_performed` | workflow | Verify project was onboarded |
| `onboarding` | workflow | First-time project onboarding |
| `list_memories` | memory | List available project memories |
| `read_memory` | memory | Load project knowledge / conventions |
| `write_memory` | memory | Persist new conventions discovered |
| `read_file` | file | Read any file in the project |
| `list_dir` | file | Browse directory tree |
| `find_file` | file | Locate files by name/pattern |
| `search_for_pattern` | file | Grep-like search across project |
| `get_symbols_overview` | symbol | Get top-level class/method list of a file |
| `find_symbol` | symbol | Search for a class/method/field globally |
| `find_referencing_symbols` | symbol | Find all usages of a symbol |
| `replace_symbol_body` | symbol | Replace a method/class body precisely |
| `insert_after_symbol` | symbol | Add code after a class/method |
| `insert_before_symbol` | symbol | Add code before a class/method |
| `create_text_file` | file | Create or overwrite a file |
| `replace_content` | file | Regex-based content replacement |
| `execute_shell_command` | cmd | Run shell commands (build, lint, test) |
| `think_about_collected_information` | workflow | Verify completeness before coding |
| `think_about_whether_you_are_done` | workflow | Verify task is truly done |

---

## Steps

### 1. ⛔ PROJECT ONBOARDING CHECK (HALT if failed)

```
check_onboarding_performed()
```
- If NOT onboarded → run `onboarding()` first.
- After onboarding completes → `list_memories()` to review what was indexed.

---

### 2. ⛔ MANDATORY COMPLIANCE CHECK (HALT if failed)

Read `openspec/mapping/artifact_context_modular.yml` → section `apply`:

```
read_file("openspec/mapping/artifact_context_modular.yml")
```

**Step 2a — Load `required_skills` (HALT if missing):**
For EACH path listed in `required_skills`:
- `find_file(<path>)` → if NOT found → **HALT IMMEDIATELY**:
  ```
  ❌ HALT: Required standard/skill not found: <path>
  Cannot generate code without project standards.
  ```
- If found → `read_file(<path>)` → load into context
- Display: `✅ Standard loaded: <path>`

**Step 2b — Load `required_rules` (HALT if missing):**
Same process using `find_file` + `read_file`.

**Step 2c — Load `context` (soft — warn if missing):**
```
list_dir("base_knowledge/structures/apply")
list_dir("base_knowledge/structures/propose")
```
For each file found → `read_file(<path>)`. WARN if empty, do NOT HALT.

**Step 2d — Load additional standards:**
```
read_file("base_knowledge/structures/overview_system.md")
list_dir("base_knowledge/common_rules")
```
For each file in `common_rules/` → `read_file(<path>)`.

**Step 2e — Load Serena memories for this project:**
```
list_memories()
```
Read any memory relevant to: coding conventions, base classes, architecture patterns.

> **CRITICAL**: MUST NOT proceed to Step 3 until ALL `required_rules`, `required_skills`, and relevant memories are loaded.

---

### 3. Read Change Artifacts

```
read_file("openspec/changes/<name>/proposal.md")
read_file("openspec/changes/<name>/srs.md")
read_file("openspec/changes/<name>/design.md")
read_file("openspec/changes/<name>/tasks.md")
```
Also:
```
list_dir("openspec/changes/<name>/specs")
```
→ `read_file` each spec file found.

**If CR variant** (detect by `find_file("openspec/changes/<name>/current-code-logic.md")`):
```
read_file("openspec/changes/<name>/current-code-logic.md")
read_file("openspec/changes/<name>/compare-logic.md")
```

---

### 4. Determine Feature Profile

Use `think_about_collected_information()` to verify everything is loaded, then cross-reference:

| Dimension | Options |
|-----------|---------|
| Mode | `CRUD` / `REPORT` |
| Entity base | `BaseFieldEntity` / `BaseFieldApprovableEntity` / `BaseBoEntity` |
| Service pattern | `IScoped` / `BaseCrossReportService` |
| Feature type | `NEWBUILD` / `MAINTENANCE` |

**Discover actual base classes in codebase:**
```
find_symbol("BaseFieldEntity")
find_symbol("BaseController")
find_symbol("IScoped")
```
→ Read found files to verify signatures before use.

**Show locked profile** before implementing. Example:
```
🔒 Feature Profile Locked:
   Mode:         CRUD
   Entity base:  BaseFieldEntity
   Service:      IScoped
   Type:         NEWBUILD
```

---

### 5. Get Apply Instructions

```bash
openspec instructions apply --change "<name>" --json
```
(via `execute_shell_command`)

---

### 6. Implement Tasks (Serena-powered loop)

Loop through tasks in `tasks.md`. For each unchecked task (`- [ ]`):

#### a. Understand the target

```
# Find the target file / class if it already exists
find_symbol("<TargetClassName>")
find_file("<target-file-path>")
```

If file exists → `get_symbols_overview("<file>")` to understand current structure.

#### b. Load convention file BEFORE generating

```
read_memory("<convention-name>")
# or
read_file("<convention-file-path>")
```

Example: for an Entity task → `read_memory("dotnet-entity-convention")`.

#### c. Check existing usages for pattern reference

```
find_referencing_symbols("<BaseClassName>")
```
→ Pick an existing similar class as a reference to match style exactly.

#### d. Generate code

For **new files**:
```
create_text_file("<relative/path/to/File.cs>", "<content>")
```

For **modifying existing class bodies** (preferred over full file rewrite):
```
replace_symbol_body("<MethodOrClassName>", "<file>", "<new_body>")
# or
insert_after_symbol("<ExistingSymbol>", "<file>", "<new_code>")
```

For **targeted text replacement** (e.g., adding a line in a block):
```
replace_content("<file>", "<regex_pattern>", "<replacement>")
```

> ✅ **Serena-first rule**: Always prefer `replace_symbol_body` / `insert_after_symbol` over full file overwrites. Only use `create_text_file` for new files.

#### e. Per-task cross-check (mandatory)

After writing each file:
```
get_symbols_overview("<written-file>")
```
Verify:
- ✓ Entity: correct base class, `[Table]`/`[Column]` attributes, namespace
- ✓ Service: `IScoped`, DI trực tiếp (KHÔNG dùng `IServiceProvider`)
- ✓ Controller: `BaseController`, `[Authorize]`, thin principle
- ✓ DTO: correct base class (`FilterRequest`/`BaseResponse`)
- ✓ Convention memory checklist passes
- If mismatch → fix with `replace_symbol_body` or `replace_content` before continuing

#### f. Mark task done

```
replace_content("openspec/changes/<name>/tasks.md", "- \\[ \\] <task-text>", "- [x] <task-text>")
```

---

### 7. Post-Implementation Verification

```
think_about_whether_you_are_done()
```

Then run build/lint if available:
```
execute_shell_command("dotnet build")
# or
execute_shell_command("ng build --configuration=production")
```

Fix any compile errors using:
```
find_symbol("<ErrorSymbol>")
replace_symbol_body("<SymbolWithError>", "<file>", "<fixed_body>")
```

---

### 8. Persist New Knowledge (if applicable)

If new patterns or conventions were discovered during implementation:
```
write_memory("<convention-name>", "<markdown content>")
```
Example: new base class pattern, new DI registration pattern.

---

### 9. Show Completion Status

```bash
openspec status --change "<name>"
```
(via `execute_shell_command`)

Prompt: "Run `/mf-review <name>` to generate tracking artifacts."

---

## Guardrails

- MUST run `check_onboarding_performed` FIRST, before any other step
- MUST use `list_memories` + `read_memory` to load project conventions — NOT assume from context
- MUST use `find_symbol` to locate existing base classes — NEVER hardcode signatures from memory
- MUST prefer `replace_symbol_body` / `insert_after_symbol` over full file rewrites
- MUST determine Feature Profile BEFORE writing any code
- MUST cross-check EVERY task output via `get_symbols_overview` after writing
- MUST use `think_about_collected_information` before coding and `think_about_whether_you_are_done` after
- If `design.md` conflicts with loaded knowledge → follow loaded knowledge (memories + standard files)
- MUST NOT invent logic outside design/specs
