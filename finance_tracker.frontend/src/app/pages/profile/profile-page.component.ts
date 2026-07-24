import { Component, inject } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { UiShellStateService } from '../../core/ui-shell-state.service';

/** Sample row — replace with GET api/users/me when integrating. */
const MOCK_PROFILE = {
  id: 'a1b2c3d4-e5f6-7890-abcd-ef1234567890',
  email: 'alex.morgan@example.com',
  username: 'alexmorgan',
  firstName: 'Alex',
  lastName: 'Morgan',
  phoneNumber: '',
} as const;

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './profile-page.component.html',
})
export class ProfilePageComponent {
  private readonly fb = inject(NonNullableFormBuilder);
  protected readonly shell = inject(UiShellStateService);

  protected readonly account = { ...MOCK_PROFILE };

  protected readonly form = this.fb.group({
    username: this.fb.control(MOCK_PROFILE.username, {
      validators: [Validators.required, Validators.minLength(3)],
    }),
    firstName: this.fb.control(MOCK_PROFILE.firstName, { validators: [Validators.required] }),
    lastName: this.fb.control(MOCK_PROFILE.lastName),
    phoneNumber: this.fb.control(MOCK_PROFILE.phoneNumber),
  });

  protected onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    // API: PUT api/users/update — UpdateInfoRequestDto
  }
}
