import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { notifyError, notifySaved } from '../../notify';
import { ApiService, Category } from '../../services/api.service';

@Component({
  selector: 'app-machine-master-add',
  imports: [FormsModule, RouterLink],
  templateUrl: './machine-master-add.component.html',
  styleUrl: './machine-master-add.component.scss'
})
export class MachineMasterAddComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);

  groups = signal<Category[]>([]);
  categoryId = 0;
  name = '';
  machineType = '';
  summary = '';
  typicalUse = '';
  description = '';
  busy = signal(false);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.api.getCategories().subscribe({
      next: (rows) => {
        this.groups.set(rows);
        if (rows[0] && !this.categoryId) {
          this.categoryId = rows[0].id;
        }
      },
      error: () => this.error.set('Could not load equipment groups.')
    });
  }

  save(): void {
    if (!this.categoryId) {
      this.error.set('Choose an equipment group first.');
      return;
    }
    if (!this.name.trim()) {
      this.error.set('Enter a machine type name.');
      return;
    }

    this.busy.set(true);
    this.error.set(null);
    this.api.createEquipmentItem({
      categoryId: this.categoryId,
      name: this.name,
      machineType: this.machineType || this.name,
      summary: this.summary || undefined,
      typicalUse: this.typicalUse || undefined,
      description: this.description || undefined
    }).subscribe({
      next: async (row) => {
        this.busy.set(false);
        await notifySaved('Machine type saved', `${row.name} is now on the public Equipment page.`);
        void this.router.navigate(['/dashboard/masters/machines']);
      },
      error: (err: HttpErrorResponse) => {
        this.busy.set(false);
        const message = typeof err.error?.message === 'string'
          ? err.error.message
          : 'Could not save that machine type.';
        this.error.set(message);
        void notifyError(message);
      }
    });
  }
}
