import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { notifyError, notifySaved } from '../../notify';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ApiService);

  showPassword = signal(false);
  submitting = signal(false);
  error = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(120)]],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', [Validators.required, Validators.maxLength(40)]],
    company: [''],
    city: [''],
    password: ['', [Validators.required, Validators.minLength(8)]],
    confirm: ['', [Validators.required]]
  });

  submit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      this.error.set('Complete the required fields. Password must be at least 8 characters.');
      return;
    }

    const value = this.form.getRawValue();
    if (value.password !== value.confirm) {
      this.error.set('Password and confirmation do not match.');
      return;
    }

    this.submitting.set(true);
    this.error.set(null);
    this.api.createRegistration({
      fullName: value.fullName,
      email: value.email,
      phone: value.phone,
      company: value.company || undefined,
      city: value.city || undefined,
      password: value.password
    }).subscribe({
      next: async () => {
        this.submitting.set(false);
        this.form.reset();
        await notifySaved('Registration saved', 'Your details are stored. An administrator can see them after login.');
      },
      error: (err: HttpErrorResponse) => {
        this.submitting.set(false);
        const message = err.status === 409
          ? 'That email is already registered.'
          : 'Could not save your details. Confirm the API and MySQL are running.';
        this.error.set(message);
        void notifyError(message);
      }
    });
  }
}
