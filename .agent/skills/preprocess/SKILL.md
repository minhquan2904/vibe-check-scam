---
name: preprocess
description: Pre-process URD into structured specification ready for OpenSpec ingestion. Use when user wants to convert raw requirements into a structured pre_openspec.md file.
---

1. **Invoke wf-pre-openspec skill**
   Read and rigorously follow the instructions below to pre-process a raw URD (User Requirements Document) into a clean, structured, and unambiguous specification file.

I'll create:
- OpenSpec change directory via CLI
- `pre_openspec.md` — structured requirements in Vietnamese, ready for `/wf_openspec`
- Feature type classification: **NEWBUILD** or **MAINTENANCE**

When ready to generate OpenSpec artifacts, prompt user: "Run `/wf_openspec <feature-name>`"

---

**Input**: User may provide URD source:

| Source Type | How to Identify | How to Collect |
|-------------|----------------|----------------|
| **Confluence page** | User provides Confluence URL or page ID | Read via Confluence MCP server (`get_page`, `search`) |
| **Plain text / notes** | User pastes raw text or provides a local file path | Read directly from message or file system |
| **External URL** | User provides a non-Confluence URL | Fetch URL content |

**Output**:

| Property | Value |
|----------|-------|
| **File** | `openspec/changes/<feature-name>/pre_openspec.md` |
| **Format** | Strict Markdown with `###` headings per requirement |
| **Language** | Vietnamese (for ALL content except IDs and technical terms) |
| **Feature Type** | `NEWBUILD` or `MAINTENANCE` (written at top of output file) |

---

**Steps**

1. **Identify URD source, derive feature name, and scaffold change**

   If the user has not provided a clear URD source, ask:
   > "Please provide the URD source:
   > - Confluence page URL or ID
   > - File path to a local document
   > - Or paste the raw requirements text"

   a. **Read URD content**:

      | Source | Action |
      |--------|--------|
      | Confluence page | Extract page ID from URL → `confluence_get_page(page_id)` |
      | Plain text / file | Read from file system or user message |
      | External URL | Fetch via `read_url_content` |

   b. **Derive feature name**: Extract from URD Use Case Name → convert to kebab-case (e.g., "Mở tài khoản HKD" → `mo-tai-khoan-hkd`)

   c. **Classify feature type** — multi-signal classification:

      **Signal 1 (Primary)**: Search `base_knowledge/structures/features.md` for keyword matches.
      - Extract 2-3 keywords from URD Use Case Name (e.g., "tất toán", "tiết kiệm", "chuyển khoản")
      - Use `grep_search` on `features.md` with each keyword (case-insensitive)
      - If keywords match an existing module → candidate for `MAINTENANCE`

      **Signal 2 (Confirmation)**: If Signal 1 matched, verify by checking if the matched module has relevant handler/controller files:
      - Use `grep_search` on the matched service's `module/` directory for related class names
      - If existing handlers found → confirmed `MAINTENANCE`
      - If no relevant code found → still `NEWBUILD` (module exists but feature is new within it)

      **Decision Table**:
      ```
      | features.md match? | Code exists? | Classification |
      |--------------------|-------------|----------------|
      | Yes                | Yes         | MAINTENANCE    |
      | Yes                | No          | NEWBUILD (new sub-feature in existing module) |
      | No                 | —           | NEWBUILD       |
      ```

      **Output**: Write classification result + reasoning to `pre_openspec.md` header:
      ```
      > **Type**: NEWBUILD | MAINTENANCE
      > **Matched module**: {service}/{module} (if MAINTENANCE)
      > **Reason**: {brief justification}
      ```

      **Fallback rules**:
      - If `features.md` does not exist → warn user to run `/wf_learn_base_code` and default to NEWBUILD
      - Token-saving: Do NOT read full source files — only use `grep_search` for class/file names

   d. **Classify transaction flow type** — read `base_knowledge/structures/propose/knowledge_transaction_flow.md` section **"Phân biệt từ URD / Mockup UI"**:
       - Nếu URD có màn xác nhận thông tin (review fees, beneficiary) trước OTP → **Financial Flow** (Init → AuthMethod → Confirm)
       - Nếu URD hiện OTP trực tiếp từ màn nhập liệu → **Non-Financial Flow** (Init → Confirm)
       - Ghi kết quả vào `pre_openspec.md` ở mục Assumptions

   e. **Create OpenSpec change directory**:
      ```bash
      openspec new change "<feature-name>"
      ```
      - If change already exists → skip creation, continue
      - This ensures the output directory `openspec/changes/<feature-name>/` exists before analysis

   **IMPORTANT**: Do NOT proceed without successfully reading the URD content AND determining the feature type.

2. **Extract raw data (NO interpretation)**

   Parse the URD content and extract without modifying meaning:

   a. **Feature name** → convert to kebab-case

   b. **Actors** → identify all participants:
      - End users (mobile app, web)
      - Backend systems
      - Bank APIs / gateways
      - Third-party services

   c. **Raw requirements** → extract sentence-level requirements from:
      - Use Case basic flow
      - Alternative flows
      - Exceptional flows
      - Business rules

   d. **Integrations** → identify all mentioned external systems

   **Rules**:
   - Do NOT infer missing logic
   - Do NOT rewrite original meaning
   - Preserve strikethrough (`~~text~~`) markers — these indicate deprecated/removed requirements

3. **Normalize into structured requirements**

   Transform each raw requirement into a structured format:

   ```
   Actor → Action → Object → Condition → Result
   ```

   Then convert to Vietnamese declarative sentence:
   - Mandatory: `"Hệ thống phải <action> khi <condition>."`
   - Optional: `"Hệ thống nên <action> khi <condition>."`

   a. **Assign IDs**: `FR-001`, `FR-002`, ...

   b. **Generate short Vietnamese title** (3–6 words) for each requirement

   c. **Split compound requirements**: If a single URD sentence contains multiple distinct actions, split into separate FR entries

   d. **Move validation rules**: Extract specific validation conditions (min/max length, allowed characters, blacklists) into the body of each FR

   **Rules**:
   - Each FR MUST have a clear, testable action
   - Each FR MUST have a condition (when/trigger) if applicable
   - Strikethrough items in URD → treat as **deprecated** — do NOT include as active FRs, but note in Issues section

4. **Deduplicate and consolidate**

   a. **Merge duplicates**: If two requirements describe the same behavior with different wording, merge into one FR

   b. **Normalize wording**: Ensure consistent terminology across all FRs

   c. **Detect conflicts**: If two requirements contradict each other (e.g., "unlimited accounts" vs "single account only"):
      - Do NOT silently remove either one
      - Keep the **latest/non-strikethrough** version as the active FR
      - Add the conflict to the **Issues** section (Section 9)

   **Rules**:
   - Do NOT discard conflicting information — always document it
   - Preserve original intent from URD

5. **Enrich with banking domain requirements**

   Review the extracted requirements and add **only if missing**:

   a. **Constraints**:
      - Idempotency for create/update operations
      - Full transaction logging
      - Timeout handling for external API calls
      - Retry mechanism for failed external calls

   b. **Non-functional**:
      - Response time within 5 seconds
      - High availability

   c. **Security**:
      - Request authentication
      - Sensitive data encryption

   d. **Integrations** (add if referenced but not explicitly listed):
      - Bank API (transfer, inquiry)
      - OTP service
      - Notification service

   **Rules**:
   - Do NOT override existing requirements
   - Only ADD missing production-critical requirements
   - Mark enriched items clearly so they are distinguishable from URD-sourced items

6. **Score quality and detect issues**

   a. **Quality scoring** — evaluate the normalized requirements:

      | Criteria | Range | What to evaluate |
      |----------|-------|------------------|
      | Clarity | 0–25 | Are requirements unambiguous? |
      | Completeness | 0–25 | Are all flows covered (happy, error, edge)? |
      | Consistency | 0–25 | Do requirements contradict each other? |
      | Testability | 0–25 | Can each FR be verified with a test? |

      - If total score **< 70**: MUST populate Open Questions (Section 7)
      - If total score **< 50**: MUST ask user for clarification BEFORE continuing

   b. **Issue detection** — scan for problems:

      | Issue Type | What to detect |
      |------------|---------------|
      | **Conflict** | Same concept with different values (e.g., 10MB vs 5MB) |
      | **Missing** | No error handling, no edge case, missing integration detail |
      | **Ambiguity** | Vague words: "nhanh", "bảo mật", "phù hợp" without measurable criteria |
      | **Risk** | Banking-sensitive gaps: no idempotency, no logging, no retry |

      For each issue found, create an `ISSUE-XXX` entry with:
      - Type (Conflict / Missing / Ambiguity / Risk)
      - Description
      - Impact
      - Suggestion

7. **Finalize output file**

   Write the final `pre_openspec.md` to `openspec/changes/<feature-name>/pre_openspec.md` using this **exact structure**:

   ```markdown
   # Feature: <kebab-case-name>

   > **Type**: NEWBUILD | MAINTENANCE

   ---

   ## 1. Actors
   - <Actor>: <Mô tả>

   ---

   ## 2. Functional Requirements

   ### FR-001: <Tiêu đề ngắn>
   Hệ thống phải <hành động> khi <điều kiện>.
   - <Chi tiết validation / quy tắc nếu có>

   ### FR-002: <Tiêu đề ngắn>
   ...

   ---

   ## 3. Non-functional Requirements
   - Hệ thống phải <ràng buộc đo lường được>.

   ---

   ## 4. Constraints
   - Hệ thống phải <ràng buộc>.

   ---

   ## 5. Integrations
   - <Hệ thống>: <Mục đích>

   ---

   ## 6. Assumptions
   - <Giả định>

   ---

   ## 7. Open Questions
   - <Câu hỏi>

   ---

   ## 8. Quality Score

   | Tiêu chí | Điểm |
   |----------|------|
   | Rõ ràng | /25 |
   | Đầy đủ | /25 |
   | Nhất quán | /25 |
   | Kiểm thử được | /25 |
   | **Tổng** | **/100** |

   ---

   ## 9. Inconsistencies & Issues

   ISSUE-XXX:
   - Loại: <Conflict | Missing | Ambiguity | Risk>
   - Mô tả: <...>
   - Ảnh hưởng: <...>
   - Đề xuất: <...>
   ```

   **Finalization rules**:
   - ALL descriptions MUST be in Vietnamese
   - Use declarative style: `"Hệ thống phải..."` / `"Hệ thống nên..."`
   - MUST follow the exact section structure above
   - MUST include `> **Type**: NEWBUILD` or `> **Type**: MAINTENANCE` at the top
   - No duplicate requirements
   - No ambiguous wording — if ambiguous, move to Open Questions
   - Missing data → use "N/A"

---

**Output**

After creating the file, summarize:
- Feature name and file location
- Feature type: **NEWBUILD** or **MAINTENANCE** (with reason)
- Number of Functional Requirements extracted
- Number of Issues detected
- Quality Score (total)
- Key issues or open questions that need resolution
- Prompt: "Ready for next step: `/wf_openspec <feature-name>`"

---

**Guardrails**

- Do NOT generate code
- Do NOT invent business logic not present in the URD
- Do NOT remove original intent from URD
- Do NOT silently discard conflicting requirements — always document in Issues
- Strikethrough items in URD are **deprecated** — do NOT include as active FRs
- If URD content cannot be retrieved, STOP and ask the user for an alternative source
- Output file language is **Vietnamese** for all content except IDs (`FR-001`, `ISSUE-001`) and technical terms (`VABGW`, `Core Banking`)
- Feature type classification MUST use directory existence checks only — do NOT read file content for classification (token saving)
- If `openspec new change` fails because change already exists, skip and continue
