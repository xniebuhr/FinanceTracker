import { Component, inject } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { newPasswordMatchValidator } from '../../core/validators/password-match.validator';
import { UiShellStateService } from '../../core/ui-shell-state.service';

@Component({
  selector: 'app-settings-page',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './settings-page.component.html',
  styleUrl: './settings-page.component.css',
})
export class SettingsPageComponent {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly router = inject(Router);
  protected readonly shell = inject(UiShellStateService);

  protected deleteStep: 'idle' | 'confirm' = 'idle';

  protected readonly passwordForm = this.fb.group(
    {
      currentPassword: this.fb.control('', { validators: [Validators.required] }),
      newPassword: this.fb.control('', {
        validators: [Validators.required, Validators.minLength(6)],
      }),
      confirmNewPassword: this.fb.control('', { validators: [Validators.required] }),
    },
    { validators: [newPasswordMatchValidator] },
  );

  protected onPasswordSubmit(): void {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }
    // API: POST api/auth/change-password or POST api/users/change-password — ChangePasswordRequestDto
  }

  protected beginDelete(): void {
    this.deleteStep = 'confirm';
  }

  protected cancelDelete(): void {
    this.deleteStep = 'idle';
  }

  protected confirmDeleteAccount(): void {
    // API: DELETE api/auth/delete
    this.shell.signOut();
    void this.router.navigateByUrl('/');
  }
}
