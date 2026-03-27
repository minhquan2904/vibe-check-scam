# Standards

This directory contains development standards organized into two layers:

## Structure

```text
standards/
├── base/                        # Tech-agnostic base skills (methodology, patterns)
│   └── <skill-name>/SKILL.md    # Generic methodology — any project can inherit
├── <skill-name>/                # Project-level domain-specific skills
│   └── <extension-files>.md     # Tech-specific content (.NET, Angular, Oracle)
└── _standards-catalog.md        # This file
```

## Project-Level Skills (active)

| Folder | Type | Description |
|--------|------|-------------|
| `angular-code-generation/` | Generation | Angular 17 component/module generation |
| `angular-convention-checker/` | Scanner | Angular convention compliance checker |
| `dotnet-code-generation/` | Generation | .NET 8 code generation standards |
| `dotnet-convention-checker/` | Scanner | .NET convention compliance checker |
| `database-design/` | Knowledge | Oracle DDL & data modeling patterns |
| `contract-management/` | Domain | Contract management business patterns |
| `state-command-patterns/` | Domain | State machine & Command pattern implementation |
| `api-patterns/` | Override | REST API patterns (extends `base/api-patterns`) |
| `fixbug/` | Override | Debug patterns (extends `base/fixbug`) |
