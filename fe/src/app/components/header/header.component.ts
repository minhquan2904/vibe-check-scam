import { Component } from '@angular/core';

@Component({
  selector: 'vc-header',
  standalone: true,
  template: `
    <header class="vc-header">
      <div class="logo">
        <span class="gradient-text">VIBE</span> CHECK
      </div>
      <div class="status-badge glass-card">
        <div class="pulse"></div>
        AI Guard Active
      </div>
    </header>
  `,
  styles: `
    .vc-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1.5rem 2rem;
      max-width: 1200px;
      margin: 0 auto;
      width: 100%;
      box-sizing: border-box;
    }
    .logo {
      font-size: 1.5rem;
      font-weight: 800;
      letter-spacing: -0.05em;
    }
    .status-badge {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0.5rem 1rem;
      border-radius: 2rem;
      font-size: 0.8rem;
      font-weight: 500;
      color: var(--text-secondary);
    }
    .pulse {
      width: 8px;
      height: 8px;
      background: var(--success);
      border-radius: 50%;
      box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7);
      animation: pulse-green 2s infinite;
    }
    @keyframes pulse-green {
      0% { box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7); }
      70% { box-shadow: 0 0 0 10px rgba(16, 185, 129, 0); }
      100% { box-shadow: 0 0 0 0 rgba(16, 185, 129, 0); }
    }
  `
})
export class HeaderComponent {}
