import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { notifyError, notifySaved } from '../../notify';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ApiService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  showPassword = signal(false);
  submitting = signal(false);
  error = signal<string | null>(null);
  left = signal(2);
  right = signal(3);

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    captcha: ['', [Validators.required]],
    remember: [false]
  });

  constructor() {
    this.refreshCaptcha();
  }

  captchaPrompt(): string {
    return `${this.left()} + ${this.right()}`;
  }

  refreshCaptcha(): void {
    this.left.set(1 + Math.floor(Math.random() * 9));
    this.right.set(1 + Math.floor(Math.random() * 9));
    this.form.patchValue({ captcha: '' });
  }

  submit(): void {
    this.form.markAllAsTouched();
    if (this.form.controls.email.invalid || this.form.controls.password.invalid) {
      this.error.set('Enter a valid email and a password of at least 8 characters.');
      return;
    }

    const answer = Number(this.form.controls.captcha.value);
    if (!Number.isFinite(answer) || answer !== this.left() + this.right()) {
      this.error.set(`Captcha is incorrect. Solve ${this.captchaPrompt()}.`);
      this.refreshCaptcha();
      return;
    }

    this.submitting.set(true);
    this.error.set(null);
    const value = this.form.getRawValue();
    this.api.login({ email: value.email, password: value.password }).subscribe({
      next: async (user) => {
        this.auth.signIn(user, value.remember);
        this.submitting.set(false);
        await notifySaved('Signed in', `${user.fullName} · ${user.role} · ${user.userType}`);
        void this.router.navigateByUrl('/dashboard');
      },
      error: (err: HttpErrorResponse) => {
        this.submitting.set(false);
        this.refreshCaptcha();
        const message = err.status === 401
          ? 'Email or password was not found in the database. Register first, or use the seeded admin account.'
          : 'Could not reach the login API. Confirm the API and MySQL are running.';
        this.error.set(message);
        void notifyError(message);
      }
    });
  }
}
