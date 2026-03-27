# Base Skills Summary

> Tóm tắt base skills dùng chung cho mọi dự án.
> Mỗi skill chỉ chứa methodology/framework generic — nội dung tech-specific nằm ở project level.

---

## 📋 Methodology & Process (3 skills)

### 1. `clean-code`
Coding standards: SRP, DRY, KISS, YAGNI, Boy Scout Rule. Naming, function design, AI coding style.

### 2. `code-review-checklist`
Review checklist: correctness, security (AI-specific), performance, quality, testing, documentation.

### 3. `tdd-workflow`
RED-GREEN-REFACTOR cycle. AAA pattern. AI-augmented TDD patterns.

---

## 🔧 Dev Practices (5 skills)

### 4. `fixbug`
6-phase bug fix: Reproduce → Isolate → Hypothesize → Fix → Verify → Harden.
- **Project extension**: `debug-patterns.md` (tech-specific debug patterns)

### 5. `testing-patterns`
Testing pyramid, AAA pattern, mocking. Adaptable to any test framework.

### 6. `security-compliance-checker`
OWASP scan methodology: rule extraction → approval → scan → report.

### 7. `api-patterns`
REST API design principles: resource naming, HTTP methods, status codes, response envelope.
- **Project extension**: `rest.md`, `response.md`, `auth.md`, `documentation.md`

### 8. `database-design`
Database design: data modeling, normalization, audit columns, sequences.

---

## 📦 Oracle Database (3 skills)

### 9. `oracle-ddl-generation`
Oracle DDL generation: naming conventions, table/index/sequence scripts.

### 10. `oracle-package-crud-generation`
Oracle PL/SQL CRUD package generation.

### 11. `oracle-package-report-generation`
Oracle PL/SQL Report package generation.

---

## 📁 Project-Level Extensions

Khi base skill cần nội dung tech-specific, file override nằm tại project-level `standards/<skill-name>/`:

| Base Skill | Project Extension File | Nội dung |
|-----------|----------------------|----------|
| `fixbug` | `*-debug-patterns.md`, `cross-layer-debug.md` | Tech-specific debug patterns |
| `api-patterns` | `rest.md`, `response.md`, `auth.md` | REST convention, auth, Swagger |
