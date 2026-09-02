import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);

  showPassword = signal(false);
  submitting = signal(false);
  signedIn = signal(false);
  error = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    remember: [false]
  });

  submit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      this.error.set('Enter a valid email and a password of at least 8 characters.');
      return;
    }

    this.submitting.set(true);
    this.error.set(null);

    // Local portal sign-in for the marketing site (no auth API yet).
    window.setTimeout(() => {
      this.submitting.set(false);
      this.signedIn.set(true);
    }, 700);
  }

  signOut(): void {
    this.signedIn.set(false);
    this.form.reset({ email: '', password: '', remember: false });
  }
}
