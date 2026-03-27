## ADDED Requirements

### Requirement: Query current gas price via Gemini LLM
The system SHALL provide an API endpoint to retrieve the current gas price by querying the Gemini API.

#### Scenario: Successful gas price retrieval
- **WHEN** user makes a valid request to the gas price endpoint
- **THEN** system calls the Gemini API to fetch the current gas price
- **AND** system parses the response and returns a structured JSON object containing the price and timestamp

#### Scenario: External dependency failure
- **WHEN** the system attempts to call the Gemini API
- **AND** the API times out or returns an error response
- **THEN** the system SHALL return an appropriate error code (e.g., HTTP 503) indicating the external service is unavailable
