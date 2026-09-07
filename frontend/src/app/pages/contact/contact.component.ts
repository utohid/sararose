import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ApiService, Category, Company, EquipmentSummary } from '../../services/api.service';

@Component({
  selector: 'app-contact',
  imports: [ReactiveFormsModule],
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.scss'
})
export class ContactComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);

  company = signal<Company | null>(null);
  categories = signal<Category[]>([]);
  equipment = signal<EquipmentSummary[]>([]);
  submitting = signal(false);
  submitted = signal(false);
  error = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(120)]],
    company: [''],
    phone: ['', [Validators.required, Validators.maxLength(40)]],
    email: ['', [Validators.required, Validators.email]],
    categoryId: ['' as string],
    machineType: [''],
    siteLocation: [''],
    requirement: ['', [Validators.required, Validators.minLength(12), Validators.maxLength(2000)]]
  });

  ngOnInit(): void {
    this.api.getCompany().subscribe({
      next: (c) => this.company.set(c),
      error: () => this.error.set('Contact details could not be loaded.')
    });
    this.api.getCategories().subscribe({
      next: (rows) => this.categories.set(rows)
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

    this.route.queryParamMap.subscribe((params) => {
      const machine = params.get('machine');
      const category = params.get('category');
      if (machine) {
        this.form.patchValue({ machineType: machine });
      }
      if (category) {
        this.form.patchValue({ categoryId: category });
      }
    });
  }

  machineOptions(): EquipmentSummary[] {
    const categoryId = this.form.controls.categoryId.value;
    const rows = this.equipment();
    if (!categoryId) {
      return rows;
    }

    const id = Number(categoryId);
    return rows.filter((item) => item.categoryId === id);
  }

  submit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      this.error.set('Please complete the required fields before sending.');
      return;
    }

    this.submitting.set(true);
    this.error.set(null);
    const value = this.form.getRawValue();
    this.api.createEnquiry({
      fullName: value.fullName,
      company: value.company || undefined,
      phone: value.phone,
      email: value.email,
      categoryId: value.categoryId ? Number(value.categoryId) : null,
      machineType: value.machineType || undefined,
      siteLocation: value.siteLocation || undefined,
      requirement: value.requirement
    }).subscribe({
      next: () => {
        this.submitting.set(false);
        this.submitted.set(true);
        this.form.reset();
      },
      error: () => {
        this.submitting.set(false);
        this.error.set('The enquiry could not be stored. Confirm the API and MySQL are running, then try again.');
      }
    });
  }
}
