# Vibe Coding: Check Scam Project

Welcome to the **Check Scam** project! This repository is built using the **Vibe Coding** methodology, leveraging **OpenSpec** for dynamic requirement specifications and the **Antigravity Kit** (AI Agent Assistant) to drive automated development, code scanning, and maintain system consistency.

## 🚀 Overview

This project embodies an AI-assisted software development workflow. By utilizing LLM-based agents (Antigravity Kit), we automate tasks ranging from requirement analysis, database generation, and API building, to code review and security scanning.

- **Methodology**: Vibe Coding (LLM-driven programming)
- **Spec Management**: OpenSpec
- **AI Tooling**: Antigravity Kit

## 📂 Project Structure

The project features a highly modular structure to strictly isolate the Frontend, Backend, business design documents, and Agent configurations:

- `api/`: Backend source code (built with C# ASP.NET Core). Core API services are located in `api/api-vibe`.
- `web/`: Frontend Web source code (Angular).
- `openspec/`: Contains API specifications (`yaml`), schemas, design documents, and change proposals. The AI Agent reads OpenAPI definitions and specs here to comprehend tasks before generating code.
- `base_knowledge/`: The central knowledge base. It contains coding standards, architectural guidelines, and security/convention checker rules that the AI Agent MUST strictly abide by.
- `.agent/`: Defines Workflows and Skills for the Antigravity Kit. Assists the agent in executing tasks such as `/table-gen` (generating PostgreSQL/Oracle DDLs), scanning .NET sources, etc.

## 🤖 Antigravity & OpenSpec Integration

In this project, the programming workflow is transformed:

1. **Define**: Write and specify requirements and API schemas inside `openspec/`.
2. **Guardrails**: Apply strict rules from the `base_knowledge/` directory (e.g., `PRJ-03-dotnet-scan-rule`, `PRJ-04-angular-scan-rule`).
3. **Command**: Command the Antigravity Agent via slash commands. The agent will automatically read the specs, scan the rules, analyze, and generate production-ready code into `api/` or `web/`.

## 🛠️ Getting Started

### Prerequisites

- The respective .NET SDK (for the API Backend)
- Node.js & Angular CLI (for the Web Frontend)
- An environment with **Gemini Antigravity Kit** enabled/installed

### Running the Backend (API)

```bash
cd api/api-vibe
dotnet restore
dotnet run
```

### Running the Frontend (Web)

_(The Web directory is currently set up conceptually)_

```bash
cd web
npm install
npm start
```

## 🛡️ Security & Standards

All generated codebase or commits can be automatically scanned and cross-referenced against the `common_rules` located at `base_knowledge/`. The project champions Clean Code, restricts the storage of sensitive information (hardcoded secrets, tokens, connection strings) in plain text, and maintains a strict separation of concerns between application layers.

---

_This project is developed and optimized through an AI-assisted coding experience._
