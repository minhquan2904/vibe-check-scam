import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TransactionIngestRequest, IngestTrackingResponse } from './transaction-ingestion.models';

@Injectable({
  providedIn: 'root'
})
export class TransactionIngestionService {
  private apiUrl = '/api/v1/transactions/ingest';

  constructor(private http: HttpClient) {}

  ingestText(request: TransactionIngestRequest): Observable<{ data: IngestTrackingResponse }> {
    return this.http.post<{ data: IngestTrackingResponse }>(this.apiUrl, request);
  }

  getStatus(trackingId: string): Observable<{ data: IngestTrackingResponse }> {
    return this.http.get<{ data: IngestTrackingResponse }>(`${this.apiUrl}/${trackingId}`);
  }
}
