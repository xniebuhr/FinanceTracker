import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { UiShellStateService } from '../core/ui-shell-state.service';

@Component({
  selector: 'app-site-header',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './site-header.component.html',
  styleUrl: './site-header.component.css',
})
export class SiteHeaderComponent {
  protected readonly shell = inject(UiShellStateService);
}
