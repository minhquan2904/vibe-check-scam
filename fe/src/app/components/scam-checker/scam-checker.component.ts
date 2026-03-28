import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ScamService } from '../../services/scam.service';
import { CheckScamResponse } from '../../models/scam.model';

@Component({
  selector: 'vc-scam-checker',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="scam-checker-container">
      <div class="glass-card main-card">
        <h1 class="gradient-text">Scam Detector</h1>
        <p class="subtitle">Paste transaction statements to analyze risk factors</p>
        
        <div class="input-area">
          <textarea 
            [(ngModel)]="rawText" 
            placeholder="Paste raw bank statement text here..."
            [disabled]="isLoading()"></textarea>
          
          <button 
            class="btn-primary" 
            (click)="onAnalyze()" 
            [disabled]="isLoading() || !rawText">
            {{ isLoading() ? 'AI Analyzing...' : 'Analyze Now' }}
          </button>
        </div>

        <div *ngIf="isLoading()" class="loading-state">
          <div class="shimmer loading-bar"></div>
          <p>Gemini AI identifying patterns...</p>
        </div>

        <div *ngIf="result() as data" class="results-fade-in">
          <hr class="divider">
          
          <div class="analysis-header" [class.scam-detected]="data.scamAnalysis.isScam">
            <div class="risk-badge">
              {{ data.scamAnalysis.isScam ? 'HIGH RISK' : 'SECURE' }}
            </div>
            <h2>{{ data.scamAnalysis.isScam ? 'Suspicious Activity Detected' : 'No Threats Found' }}</h2>
            <p class="score">Confidence Score: {{ data.scamAnalysis.confidenceScore * 100 }}%</p>
          </div>

          <div class="reason-box">
             <strong>AI Insight:</strong> {{ data.scamAnalysis.reason }}
          </div>

          <div class="transactions-list">
             <h3>Extracted Transactions ({{ data.transactions.length }})</h3>
             <div class="tx-item" *ngFor="let tx of data.transactions">
                <code>{{ tx.rawContent }}</code>
             </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: `
    .scam-checker-container {
      max-width: 800px;
      margin: 2rem auto;
      padding: 0 1rem;
    }
    .main-card {
      padding: 3rem;
    }
    h1 { font-size: 2.5rem; margin-bottom: 0.5rem; }
    .subtitle { color: var(--text-secondary); margin-bottom: 2rem; }
    
    .input-area {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }
    textarea {
      width: 100%;
      height: 150px;
      background: rgba(0,0,0,0.2);
      border: 1px solid var(--glass-border);
      border-radius: 1rem;
      padding: 1.5rem;
      color: white;
      font-family: inherit;
      font-size: 1rem;
      resize: none;
      transition: border-color 0.3s;
    }
    textarea:focus {
      outline: none;
      border-color: var(--primary);
    }
    
    .loading-state {
      margin-top: 2rem;
      text-align: center;
      color: var(--text-secondary);
      font-size: 0.9rem;
    }
    .loading-bar {
      height: 4px;
      border-radius: 2px;
      margin-bottom: 1rem;
    }

    .analysis-header {
      margin-top: 2rem;
      padding: 1.5rem;
      border-radius: 1rem;
      background: rgba(16, 185, 129, 0.1);
      border-left: 4px solid var(--success);
    }
    .analysis-header.scam-detected {
      background: rgba(239, 68, 68, 0.1);
      border-left-color: var(--danger);
    }
    .risk-badge {
      display: inline-block;
      padding: 0.25rem 0.75rem;
      border-radius: 0.5rem;
      font-size: 0.7rem;
      font-weight: 700;
      margin-bottom: 0.5rem;
      background: var(--success);
      color: white;
    }
    .scam-detected .risk-badge { background: var(--danger); }
    
    .reason-box {
      margin-top: 1.5rem;
      padding: 1.5rem;
      background: var(--glass-bg);
      border-radius: 1rem;
      line-height: 1.6;
    }
    
    .transactions-list { margin-top: 2rem; }
    h3 { font-size: 1.1rem; color: var(--text-secondary); margin-bottom: 1rem; }
    .tx-item {
      padding: 0.75rem 1rem;
      margin-bottom: 0.5rem;
      background: rgba(255,255,255,0.02);
      border-radius: 0.5rem;
      font-size: 0.85rem;
    }
    .divider { border: 0; border-top: 1px solid var(--glass-border); margin: 2.5rem 0; }
    
    .results-fade-in {
      animation: fadeIn 0.6s ease-out;
    }
    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(10px); }
      to { opacity: 1; transform: translateY(0); }
    }
  `
})
export class ScamCheckerComponent {
  private scamService = inject(ScamService);
  
  rawText = '';
  isLoading = signal(false);
  result = signal<CheckScamResponse | null>(null);

  onAnalyze() {
    if (!this.rawText) return;
    
    this.isLoading.set(true);
    this.result.set(null);

    this.scamService.checkScam(this.rawText).subscribe({
      next: (data) => {
        this.result.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.isLoading.set(false);
        // Fallback for demo
        this.result.set({
           transactions: [{ rawContent: 'Sample Error Response' }],
           scamAnalysis: { isScam: false, confidenceScore: 0, reason: 'Failed to connect to backend api.' }
        });
      }
    });
  }
}
