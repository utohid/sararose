import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { confirmDelete, notifyError, notifySaved } from '../../notify';
import { ApiService, Category } from '../../services/api.service';

@Component({
  selector: 'app-equipment-master-view',
  imports: [FormsModule, RouterLink],
  templateUrl: './equipment-master-view.component.html',
  styleUrl: './equipment-master-view.component.scss'
})
export class EquipmentMasterViewComponent implements OnInit {
  private readonly api = inject(ApiService);

  rows = signal<Category[]>([]);
  selected = signal<Category | null>(null);
  name = '';
  shortName = '';
  code = '';
  summary = '';
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.reload();
  }

  select(row: Category | null): void {
    this.selected.set(row);
    this.name = row?.name ?? '';
    this.shortName = row?.shortName ?? '';
    this.code = row?.code ?? '';
    this.summary = row?.summary ?? '';
  }

  reload(): void {
    this.api.getCategories().subscribe({
      next: (rows) => {
        this.rows.set(rows);
        const current = this.selected();
        const match = current ? rows.find((row) => row.id === current.id) : undefined;
        this.select(match ?? rows[0] ?? null);
      },
      error: () => this.error.set('Could not load equipment groups. Confirm the API is running.')
    });
  }

  saveEdits(): void {
    const row = this.selected();
    if (!row || !this.name.trim()) {
      this.error.set('Enter an equipment group name.');
      return;
    }

    this.api.updateCategory(row.id, {
      name: this.name,
      shortName: this.shortName,
      code: this.code,
      summary: this.summary
    }).subscribe({
      next: async (saved) => {
        await notifySaved('Equipment group updated', `${saved.name} is saved for the public Equipment page.`);
        this.reload();
      },
      error: () => {
        this.error.set('Could not update that equipment group.');
        void notifyError('Could not update that equipment group.');
      }
    });
  }

  move(row: Category, direction: -1 | 1): void {
    this.api.updateCategory(row.id, { name: row.name, sortOrder: row.sortOrder + direction }).subscribe({
      next: () => this.reload(),
      error: () => this.error.set('Could not change that group order.')
    });
  }

  async remove(row: Category): Promise<void> {
    const result = await confirmDelete(`Remove “${row.name}” from the equipment master?`);
    if (!result.isConfirmed) {
      return;
    }

    this.api.deleteCategory(row.id).subscribe({
      next: async () => {
        this.selected.set(null);
        await notifySaved('Equipment group deleted', `${row.name} is no longer on the public list.`);
        this.reload();
      },
      error: (err: HttpErrorResponse) => {
        const message = typeof err.error?.message === 'string'
          ? err.error.message
          : 'Could not delete that equipment group.';
        this.error.set(message);
        void notifyError(message);
      }
    });
  }
}
