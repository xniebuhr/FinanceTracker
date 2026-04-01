import { Injectable, signal } from '@angular/core';

/**
 * Temporary UI-only state so the shell can show logged-in vs logged-out headers.
 * Replace with real auth when wiring the API.
 */
@Injectable({ providedIn: 'root' })
export class UiShellStateService {
  readonly isLoggedIn = signal(false);

  setLoggedIn(value: boolean): void {
    this.isLoggedIn.set(value);
  }

  toggleLoggedInPreview(): void {
    this.isLoggedIn.update((v) => !v);
  }
}
