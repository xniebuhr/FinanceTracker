import { Component } from '@angular/core';

@Component({
  selector: 'app-landing',
  standalone: true,
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.css',
})
export class LandingComponent {
  /** Mock figures for layout only — replace with API data later. */
  protected readonly snapshot = {
    income: 4200,
    expense: 3180,
  };

  protected net(): number {
    return this.snapshot.income - this.snapshot.expense;
  }

  protected formatUsd(n: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      maximumFractionDigits: 0,
    }).format(n);
  }
}
