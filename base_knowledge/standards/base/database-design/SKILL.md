---
name: database-design
description: Database design principles for Oracle. Schema design, indexing strategy, query optimization.
allowed-tools: Read, Write, Edit, Glob, Grep
---

# Database Design

> **Database design principles for Oracle banking systems.**
> **Learn to THINK, not copy SQL patterns.**

## 🎯 Selective Reading Rule

**Read ONLY files relevant to the request!** Check the content map, find what you need.

| File | Description | When to Read |
|------|-------------|--------------|
| `schema-design.md` | Normalization, PKs, relationships, Oracle types | Designing schema |
| `indexing.md` | Oracle index types, composite indexes, partitioning | Performance tuning |
| `optimization.md` | EXPLAIN PLAN, hints, N+1, query tuning | Query optimization |

---

## 🔗 Related Skills

| Need | Skill |
|------|-------|
| DDL generation | `@[skills/oracle-ddl-generation]` |
| CRUD packages | `@[skills/oracle-package-crud-generation]` |
| Report packages | `@[skills/oracle-package-report-generation]` |

---

## ⚠️ Core Principles

- Follow project naming conventions (rule `10-database-naming-convention`)
- Oracle is the ONLY database — no PostgreSQL, MySQL, etc.
- No ORM — use PL/SQL packages for data access
- All financial tables MUST have audit columns (`RECORD_STAT`, `AUTH_STAT`, `MAKER_ID`, `CHECKER_ID`, etc.)

---

## Decision Checklist

Before designing schema:

- [ ] Followed `10-database-naming-convention` naming rules?
- [ ] Checked `12-logical-data-modeling-rule` for data modeling standards?
- [ ] Planned index strategy for WHERE/JOIN columns?
- [ ] Defined relationship types and FK constraints?
- [ ] Included audit columns (maker/checker, timestamps)?
- [ ] Considered partitioning for large tables?

---

## Anti-Patterns

❌ Use ORM instead of PL/SQL packages
❌ Skip indexing on foreign keys
❌ Use `SELECT *` in production queries
❌ Store JSON when structured columns are better
❌ Ignore N+1 queries in application code
❌ Create tables without audit columns
