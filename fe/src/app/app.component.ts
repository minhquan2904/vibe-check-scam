import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './components/header/header.component';
import { ScamCheckerComponent } from './components/scam-checker/scam-checker.component';

@Component({
  selector: 'vc-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, ScamCheckerComponent],
  template: `
    <vc-header></vc-header>
    <main class="page-content">
      <vc-scam-checker></vc-scam-checker>
    </main>
    <router-outlet />
  `,
  styles: `
    .page-content {
      flex: 1;
      display: flex;
      flex-direction: column;
      justify-content: center;
      padding-bottom: 5rem;
    }
  `
})
export class AppComponent {
  title = 'vibe-check-ui';
}
