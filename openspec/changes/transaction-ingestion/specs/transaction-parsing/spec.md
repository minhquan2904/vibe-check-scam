## ADDED Requirements

### Requirement: REQ-01 Ingest Raw Text
The system SHALL provide an endpoint to receive raw transaction text from users.

#### Scenario: User submits valid raw text
- **WHEN** the user submits a non-empty raw text of a bank statement
- **THEN** the system SHALL create a tracking record with `idle` state and return the tracking ID

#### Scenario: User submits empty text
- **WHEN** the user submits empty or whitespace-only text
- **THEN** the system SHALL return an HTTP 400 Bad Request error

### Requirement: REQ-02 Asynchronous Parsing State Management
The system SHALL manage the state of the parsing task, transitioning from `idle` to `parsing` before calling the AI service, and finally to `parsed` or `error`.

#### Scenario: Successful AI parsing
- **WHEN** the AI service successfully extracts the transaction data
- **THEN** the system SHALL update the tracking record state to `parsed` and save the structured transaction data

#### Scenario: Failed AI parsing
- **WHEN** the AI service cannot extract valid data from the text or an error occurs
- **THEN** the system SHALL update the tracking record state to `error` and preserve the raw text for manual review

### Requirement: REQ-03 Target Data Extraction
The AI service SHALL extract at minimum: AccountNumber, Amount, TransactionDate, Description, and BankCode from the raw text.

#### Scenario: Complete data extraction
- **WHEN** the AI service analyzes the banking raw text
- **THEN** it SHALL return a mapped structure containing AccountNumber, Amount, TransactionDate, Description, and BankCode

### Requirement: REQ-04 User Interface for Ingestion
The frontend SHALL provide a UI for users to paste raw text and monitor the processing status.

#### Scenario: Viewing tracking status
- **WHEN** the user navigates to the transaction ingestion view
- **THEN** the frontend SHALL list the submitted ingestion tasks along with their current states (`idle`, `parsing`, `parsed`, `error`)
