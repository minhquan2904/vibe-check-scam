# Transaction Ingestion Walkthrough

## 📝 Overview
This feature introduces an automated pipeline for ingesting and tracking raw banking statements or CSV files. The AI-powered parser is initialized, processing the raw text to extract structured data such as AccountNumber, Amount, Description, TransactionDate, and BankCode.

The workflow acts primarily as a command execution with clear states to handle asynchronous LLM extraction tasks:
- `idle`: Initiated
- `parsing`: Wait for AI result
- `parsed`: Successfully structured 
- `error`: Failed to extract or AI offline

## 🛠️ Components Created

### 1. Database Layer (PostgreSQL)
- `IngestTracking` table with the Enum state `ingest_state`.

### 2. Backend (.NET Core API)
- **Entities**: `IngestTracking`
- **DTOs**: `TransactionIngestRequest`, `IngestTrackingResponse`
- **Interfaces**: `IAICoreService`, `ITransactionIngestionService`
- **Services**: `TransactionIngestionService` (manages workflow states)
- **Controllers**: `TransactionIngestController` (`POST /ingest`, `GET /{trackingId}`)

### 3. Frontend (Angular)
- **Services**: `TransactionIngestionService` (REST API integration)
- **Components**: `TransactionIngestionComponent` (Textbox view for ingestion + basic polling simulation)
- **Models**: `TransactionIngestRequest`, `IngestTrackingResponse`

## 🧪 Testing Guide
1. Run the `.NET API` and test `POST /api/v1/transactions/ingest` using Postman/Swagger with a sample block of text.
2. Note the returned `TrackingId` and query `GET /api/v1/transactions/ingest/{trackingId}` to observe real-time status transitions.
3. Serve the Angular application and navigate to the ingestion UI to paste raw statement texts and watch the tracking logic resolve.
