-- Note: Execute this on PostgreSQL database

-- Type: State of parsing process
CREATE TYPE ingest_state AS ENUM ('idle', 'parsing', 'parsed', 'error');

-- Table: IngestTracking
CREATE TABLE IngestTracking (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    RawContent TEXT NOT NULL,
    State ingest_state DEFAULT 'idle',
    FailureReason VARCHAR(1000),
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Index for searching and processing tracking jobs
CREATE INDEX idx_ingest_tracking_state ON IngestTracking(State);
CREATE INDEX idx_ingest_tracking_created_at ON IngestTracking(CreatedAt DESC);

-- Comment
COMMENT ON TABLE IngestTracking IS 'Tracking AI parsing process for transaction raw text';
COMMENT ON COLUMN IngestTracking.State IS 'Current state of the parsing process (idle, parsing, parsed, error)';

-- Alter Transaction table (Assume it exists)
-- ALTER TABLE Transaction ADD COLUMN IngestTrackingId UUID NULL REFERENCES IngestTracking(Id);

-- Sample Data
INSERT INTO IngestTracking (RawContent, State) VALUES
('012495034 BIDV -50k, chuyen khoan', 'idle'),
('990422 VCB 500000 vnđ mua my pham', 'idle'),
('Rác error', 'error');

COMMIT;
