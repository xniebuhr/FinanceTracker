import { Component, inject } from '@angular/core';

import { UiShellStateService } from '../core/ui-shell-state.service';

@Component({
  selector: 'app-site-footer',
  standalone: true,
  templateUrl: './site-footer.component.html',
  styleUrl: './site-footer.component.css',
})
export class SiteFooterComponent {
  protected readonly shell = inject(UiShellStateService);
}
