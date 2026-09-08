import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { notifyError, notifySaved } from '../../notify';
import { ApiService, Category, EquipmentSummary } from '../../services/api.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ApiService);

  showPassword = signal(false);
  submitting = signal(false);
  error = signal<string | null>(null);
  categories = signal<Category[]>([]);
  equipment = signal<EquipmentSummary[]>([]);

  form = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(120)]],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', [Validators.required, Validators.maxLength(40)]],
    company: [''],
    city: [''],
    categoryId: ['', [Validators.required]],
    machineType: ['', [Validators.required]],
    userType: ['Customer', [Validators.required]],
    role: ['User', [Validators.required]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    confirm: ['', [Validators.required]]
  });

  ngOnInit(): void {
    this.api.getCategories().subscribe({
      next: (rows) => this.categories.set(rows),
      error: () => this.error.set('Equipment types could not be loaded. Confirm the API is running.')
    });
    this.api.getEquipment().subscribe({
      next: (rows) => this.equipment.set(rows)
    });

    this.form.controls.categoryId.valueChanges.subscribe(() => {
      const allowed = this.machineOptions().map((item) => item.machineType);
      const current = this.form.controls.machineType.value;
      if (current && !allowed.includes(current)) {
        this.form.patchValue({ machineType: '' });
      }
    });
  }

  machineOptions(): EquipmentSummary[] {
    const categoryId = this.form.controls.categoryId.value;
    const rows = this.equipment();
    if (!categoryId) {
      return rows;
    }

    return rows.filter((item) => item.categoryId === Number(categoryId));
  }

  submit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      this.error.set('Complete the required fields, including equipment type and machine type. Password must be at least 8 characters.');
      return;
    }

    const value = this.form.getRawValue();
    if (value.password !== value.confirm) {
      this.error.set('Password and confirmation do not match.');
      return;
    }

    const category = this.categories().find((row) => row.id === Number(value.categoryId));
    if (!category) {
      this.error.set('Select an equipment type.');
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
      userType: value.userType,
      role: value.role,
      password: value.password,
      equipmentType: category.name,
      machineType: value.machineType
    }).subscribe({
      next: async () => {
        this.submitting.set(false);
        this.form.reset({
          userType: 'Customer',
          role: 'User',
          categoryId: '',
          machineType: '',
          company: '',
          city: '',
          password: '',
          confirm: '',
          fullName: '',
          email: '',
          phone: ''
        });
        await notifySaved(
          'Registration saved',
          'Your details are stored on the Registration table. An administrator creates a UserMaster login before you can sign in.');
      },
      error: (err: HttpErrorResponse) => {
        this.submitting.set(false);
        const message = err.status === 409
          ? (typeof err.error?.message === 'string' ? err.error.message : 'That email is already registered.')
          : (typeof err.error?.message === 'string' ? err.error.message : 'Could not save your details. Confirm the API and MySQL are running.');
        this.error.set(message);
        void notifyError(message);
      }
    });
  }
}
