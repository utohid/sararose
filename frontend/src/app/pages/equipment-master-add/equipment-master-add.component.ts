import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { notifyError, notifySaved } from '../../notify';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-equipment-master-add',
  imports: [FormsModule, RouterLink],
  templateUrl: './equipment-master-add.component.html',
  styleUrl: './equipment-master-add.component.scss'
})
export class EquipmentMasterAddComponent {
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);

  name = '';
  shortName = '';
  code = '';
  summary = '';
  busy = signal(false);
  error = signal<string | null>(null);

  save(): void {
    if (!this.name.trim()) {
      this.error.set('Enter an equipment group name.');
      return;
    }

    this.busy.set(true);
    this.error.set(null);
    this.api.createCategory({
      name: this.name,
      shortName: this.shortName || undefined,
      code: this.code || undefined,
      summary: this.summary || undefined
    }).subscribe({
      next: async (row) => {
        this.busy.set(false);
        await notifySaved('Equipment group saved', `${row.name} is now on the public Equipment page.`);
        void this.router.navigate(['/dashboard/masters/equipment']);
      },
      error: (err: HttpErrorResponse) => {
        this.busy.set(false);
        const message = typeof err.error?.message === 'string'
          ? err.error.message
          : 'Could not save that equipment group.';
        this.error.set(message);
        void notifyError(message);
      }
    });
  }
}
