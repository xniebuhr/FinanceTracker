import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/** Cross-field check: `password` and `confirmPassword` must match. */
export const passwordMatchValidator: ValidatorFn = (
  control: AbstractControl,
): ValidationErrors | null => {
  const password = control.get('password')?.value as string | undefined;
  const confirm = control.get('confirmPassword')?.value as string | undefined;
  if (!password || !confirm) {
    return null;
  }
  return password === confirm ? null : { passwordMismatch: true };
};

/** For change-password: `newPassword` and `confirmNewPassword` must match. */
export const newPasswordMatchValidator: ValidatorFn = (
  control: AbstractControl,
): ValidationErrors | null => {
  const password = control.get('newPassword')?.value as string | undefined;
  const confirm = control.get('confirmNewPassword')?.value as string | undefined;
  if (!password || !confirm) {
    return null;
  }
  return password === confirm ? null : { newPasswordMismatch: true };
};
