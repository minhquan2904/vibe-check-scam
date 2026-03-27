---
name: tasks
description: Generate implementation tasks ordered by layer and dependency with full traceability.
---

Generate the implementation task breakdown based on specs and design.

**Output**: `tasks.md` in `openspec/changes/<name>/`

---

**Input**: Change name (kebab-case). `specs/` and `design.md` must exist.

**Steps**

1. **Verify prerequisites**

   Check that `design.md` and at least one file in `specs/` exist.

2. **Read dependency artifacts**

   - `specs/**/*.md` → behavioral requirements
   - `design.md` → component mapping, class specs, package structure

3. **Load context**

   Read `openspec/mapping/artifact_context_modular.yml` → section `tasks`:
   - Resolve `context` paths → knowledge files

4. **Get tasks instructions**

   ```bash
   openspec instructions tasks --change "<name>" --json
   ```

5. **Generate `tasks.md`**

   Follow template structure. Order tasks by dependency layer:

   **Backend (.NET):**
   1. Entity + DbSet registration (data layer first)
   2. DTO Request/Response
   3. Service Interface (`I{Name}Service`)
   4. Service Implementation (`IScoped`, DI trực tiếp)
   5. Controller (`BaseController`, `[Authorize]`, FunctionConst)
   6. Mapping class (if needed)

   **Frontend (Angular 17):**
   7. DTO models (TypeScript)
   8. API Service
   9. Module + routing
   10. List Component (`signal()`, `TableConfig`)
   11. Form Component (Reactive Forms)
   12. Detail Component (if applicable)

   Each task MUST include:
   - File path to create/modify
   - Base class / extends
   - Namespace / module path
   - Implementation notes
   - Dependency on previous tasks

6. **Verify traceability**

   - ✓ Every requirement from specs maps to at least one task
   - ✓ Every component from design maps to a task
   - ✓ No orphan tasks (tasks without spec/design backing)

7. **Show status**

   Prompt: "Run `/mf-apply <name>` to start implementation."

**Guardrails**
- MUST follow layer order strictly
- MUST cover all specs and design components
- MUST maintain traceability: Requirement → Spec → Task
- Each task must be implementable independently (given dependencies)
