---
name: learn
description: Learn codebase — scan project architecture, patterns, and practices, then generate knowledge files for propose and apply phases.
---

Learn the project codebase by performing 9 sequential analysis steps, generating knowledge files that serve as context for the entire pipeline.

**Output**: 8 knowledge files + 1 features registry

---

**Input**: No arguments required. Optionally `--step <N>` to run a single step.

**Config**:
- **PROPOSE_DIR**: `base_knowledge/structures/propose/`
  > Skills 1–7 write here. Knowledge used during propose phase.
- **APPLY_DIR**: `base_knowledge/structures/apply/`
  > Skill 8 writes here. Knowledge used during apply phase.

**Tech Stack**: C# / .NET 8 / Entity Framework Core / Oracle / Angular 17

**Steps — Propose Knowledge (architecture & patterns)**

1. **Learn Architecture** → `knowledge_architecture.md`
   - Scan project directory structure (`.sln`, `.csproj` files)
   - Identify solution structure: `BOBase.Domain`, `BOBase.Infrastructure`, `modules/BOModule.*`
   - Trace standard request flow: Controller → Service → DbContext → Oracle DB
   - Document layer boundaries, namespaces, folder conventions
   - Scan patterns:
     ```
     find_by_name: Pattern="*.csproj", SearchDirectory={project_root}
     find_by_name: Pattern="*.sln", SearchDirectory={project_root}
     find_by_name: Pattern="*Controller.cs", SearchDirectory={project_root}
     ```
   - Output to: `PROPOSE_DIR`

2. **Learn Entity/DbContext Patterns** → `knowledge_entity_dbcontext.md`
   - Scan Entity Framework entities in `BOBase.Domain/Entities/`
   - Document base entity hierarchy: `BaseBoEntity`, `BaseFieldEntity`, `BaseFieldApprovableEntity`
   - Document `[Table]`, `[Column]`, `[Key]`, `[ForeignKey]` conventions
   - Scan DbContext registration patterns
   - Scan patterns:
     ```
     find_by_name: Pattern="*Entity.cs", SearchDirectory={project_root}
     grep_search: "[Table(" in BOBase.Domain
     find_by_name: Pattern="*DbContext.cs", SearchDirectory={project_root}
     ```
   - Output to: `PROPOSE_DIR`

3. **Learn Service Patterns** → `knowledge_service.md`
   - Scan service implementations in `modules/BOModule.*/Services/`
   - Document `IScoped` marker interface usage
   - Document DI pattern (constructor injection)
   - Document interface-implementation pairing (`I{Name}Service` → `{Name}Service`)
   - Scan patterns:
     ```
     find_by_name: Pattern="*Service.cs", SearchDirectory={project_root}
     grep_search: "IScoped" in modules/
     grep_search: "constructor" or "inject" patterns
     ```
   - Output to: `PROPOSE_DIR`

4. **Learn Controller Patterns** → `knowledge_controller.md`
   - Scan controllers in `modules/BOModule.*/Controllers/`
   - Document `BaseController` inheritance
   - Document `[Authorize(Roles = FunctionConst.*)]` pattern
   - Document `return Response(...)` pattern
   - Document thin controller principle
   - Scan patterns:
     ```
     find_by_name: Pattern="*Controller.cs", SearchDirectory={project_root}
     grep_search: "BaseController" in modules/
     grep_search: "FunctionConst" in modules/
     ```
   - Output to: `PROPOSE_DIR`

5. **Learn DTO Patterns** → `knowledge_dto.md`
   - Scan DTO classes in `modules/BOModule.*/DTOs/`
   - Document base DTO classes: `FilterRequest`, `BaseResponse`, `BaseProcModel`
   - Document request/response naming conventions
   - Document `[ClientTableConfig]` for report mode
   - Scan patterns:
     ```
     find_by_name: Pattern="*Request.cs", SearchDirectory={project_root}
     find_by_name: Pattern="*Response.cs", SearchDirectory={project_root}
     grep_search: "FilterRequest" in modules/
     ```
   - Output to: `PROPOSE_DIR`

6. **Learn Report Patterns** → `knowledge_report.md`
   - Scan report modules in `modules/BOModule.Report/`
   - Document `BaseCrossReportService` hierarchy
   - Document Setting → OracleParameter mapping
   - Document `EPackage` enum registration
   - Scan patterns:
     ```
     find_by_name: Pattern="*Setting.cs", SearchDirectory={project_root}
     grep_search: "BaseCrossReportService" in modules/
     grep_search: "EPackage" in BOBase.Domain
     ```
   - Output to: `PROPOSE_DIR`

7. **Learn Angular Patterns** → `knowledge_angular.md`
   - Scan Angular project structure in `src/app/`
   - Document lazy-loading module pattern
   - Document `inject()`, `signal()` usage
   - Document `TableConfig` data table pattern
   - Document shared module integration patterns
   - Scan patterns:
     ```
     find_by_name: Pattern="*.module.ts", SearchDirectory={angular_root}
     find_by_name: Pattern="*.component.ts", SearchDirectory={angular_root}
     find_by_name: Pattern="*.service.ts", SearchDirectory={angular_root}
     ```
   - Output to: `PROPOSE_DIR`

**Steps — Apply Knowledge (coding practices)**

8. **Learn Code Practices** → `knowledge_code_practices.md`
   - Scan logging patterns
   - Scan validation patterns (Data Annotations, custom validators)
   - Scan exception handling patterns (error codes, response building)
   - Scan mapping patterns (Entity ↔ DTO conversion)
   - Scan patterns:
     ```
     grep_search: "try.*catch" in modules/
     grep_search: "Response(" in modules/
     find_by_name: Pattern="*Mapper.cs", SearchDirectory={project_root}
     ```
   - Output to: `APPLY_DIR`

**Steps — Features Registry**

9. **Generate features registry** → `features.md`
   - Scan ALL `BOModule.*` directories
   - For each module: extract name, generate keywords (Vietnamese + English), write description
   - Output to: `base_knowledge/structures/features.md`
   - Format: Markdown table grouped by module
   - Used to classify NEWBUILD vs MAINTENANCE

**Verification**

10. Cross-verify: ensure ALL output files (8 knowledge + 1 features) contain NO `{placeholder}`, and class/service/module names are consistent across files.

**Guardrails**
- Each step MUST cite actual class names and file paths from the codebase
- MUST NOT guess or use placeholder names
- Steps must be run in order (each may depend on findings from previous steps)
- If codebase is too large for single conversation, use `--step <N>` flag
- Knowledge files MUST be self-contained (readable without other knowledge files)
- MUST reference `dotnet-convention-checker/` conventions when documenting patterns
