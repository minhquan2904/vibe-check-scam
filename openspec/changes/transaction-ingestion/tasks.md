## 1. Oracle DDL (Database Layer)
- [x] 1.1 Create `IngestTracking` table and modify `Transaction` table to support `IngestTrackingId`.

## 2. Backend (.NET API)
- [x] 2.1 Add `IngestTracking` Entity and update `Transaction` Entity, register to `AppDbContext`.
- [x] 2.2 Create DTOs: `TransactionIngestRequest` and `IngestTrackingResponse`.
- [x] 2.3 Define Interfaces: `ITransactionIngestionService`, `IAICoreService`.
- [x] 2.4 Implement `AICoreService`: Logic to call Gemini API using `Gemini:ApiKey` and prompt template.
- [x] 2.5 Implement `TransactionIngestionService`: Manage tracking state, delegate to `AICoreService`, and parse JSON output.
- [x] 2.6 Create `TransactionIngestionController`: Expose `POST /ingest` and `GET /{trackingId}` endpoints.

## 3. Frontend (Angular)
- [x] 3.1 Create DTO models/interfaces in TypeScript mirroring the Backend DTOs.
- [x] 3.2 Create `TransactionIngestService` to call API endpoints (`/ingest`, status polling).
- [x] 3.3 Create `TransactionIngestComponent` for the UI (Textbox/CSV Upload and Tracking Status display).
- [x] 3.4 Configure Routing to expose the new ingestion page.
