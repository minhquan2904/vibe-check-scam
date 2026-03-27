export interface TransactionIngestRequest {
  content: string;
}

export interface IngestTrackingResponse {
  trackingId: string;
  state: 'idle' | 'parsing' | 'parsed' | 'error';
  failureReason?: string;
  createdAt: string;
  updatedAt: string;
}
