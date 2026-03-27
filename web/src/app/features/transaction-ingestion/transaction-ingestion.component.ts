import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TransactionIngestionService } from './transaction-ingestion.service';
import { IngestTrackingResponse } from './transaction-ingestion.models';

@Component({
  selector: 'app-transaction-ingestion',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="ingestion-container">
      <h2>Transaction Ingestion</h2>
      <textarea [(ngModel)]="rawText" rows="10" cols="50" placeholder="Paste bank statement text here..."></textarea>
      <button (click)="submit()">Ingest Data</button>

      <div *ngIf="trackingState">
        <h3>Status: {{ trackingState.state }}</h3>
        <p>Tracking ID: {{ trackingState.trackingId }}</p>
      </div>
    </div>
  `
})
export class TransactionIngestionComponent {
  rawText: string = '';
  trackingState: IngestTrackingResponse | null = null;

  constructor(private service: TransactionIngestionService) {}

  submit() {
    if (!this.rawText) return;
    this.service.ingestText({ content: this.rawText }).subscribe(response => {
      this.trackingState = response.data;
      this.pollStatus(this.trackingState.trackingId);
    });
  }

  pollStatus(id: string) {
    // Basic polling simulation
    setInterval(() => {
      this.service.getStatus(id).subscribe(response => {
        this.trackingState = response.data;
      });
    }, 5000);
  }
}
