import { Component, inject } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { passwordMatchValidator } from '../../core/validators/password-match.validator';

@Component({
  selector: 'app-signup-page',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './signup-page.component.html',
})
export class SignupPageComponent {
  private readonly fb = inject(NonNullableFormBuilder);

  protected readonly form = this.fb.group(
    {
      email: this.fb.control('', { validators: [Validators.required, Validators.email] }),
      username: this.fb.control('', {
        validators: [Validators.required, Validators.minLength(3)],
      }),
      password: this.fb.control('', {
        validators: [Validators.required, Validators.minLength(6)],
      }),
      confirmPassword: this.fb.control('', { validators: [Validators.required] }),
      firstName: this.fb.control('', { validators: [Validators.required] }),
      lastName: this.fb.control(''),
      phoneNumber: this.fb.control(''),
    },
    { validators: [passwordMatchValidator] },
  );

  protected onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    // API: POST api/auth/register — RegisterRequestDto
  }
}
