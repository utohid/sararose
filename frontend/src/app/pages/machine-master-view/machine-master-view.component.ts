import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { confirmDelete, notifyError, notifySaved } from '../../notify';
import { ApiService, Category, EquipmentDetail, EquipmentSummary } from '../../services/api.service';

@Component({
  selector: 'app-machine-master-view',
  imports: [FormsModule, RouterLink],
  templateUrl: './machine-master-view.component.html',
  styleUrl: './machine-master-view.component.scss'
})
export class MachineMasterViewComponent implements OnInit {
  private readonly api = inject(ApiService);

  groups = signal<Category[]>([]);
  rows = signal<EquipmentSummary[]>([]);
  selected = signal<EquipmentDetail | null>(null);
  categoryId = 0;
  name = '';
  machineType = '';
  summary = '';
  typicalUse = '';
  description = '';
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.api.getCategories().subscribe({
      next: (rows) => this.groups.set(rows),
      error: () => this.error.set('Could not load equipment groups.')
    });
    this.reload();
  }

  reload(): void {
    this.api.getEquipment().subscribe({
      next: (rows) => {
        this.rows.set(rows);
        const current = this.selected();
        const match = current ? rows.find((row) => row.id === current.id) : undefined;
        const pick = match ?? rows[0];
        if (pick) {
          this.loadDetail(pick.id);
        } else {
          this.selected.set(null);
        }
      },
      error: () => this.error.set('Could not load machine types. Confirm the API is running.')
    });
  }

  choose(row: EquipmentSummary): void {
    this.loadDetail(row.id);
  }

  saveEdits(): void {
    const row = this.selected();
    if (!row || !this.name.trim() || !this.categoryId) {
      this.error.set('Enter a name and choose an equipment group.');
      return;
    }

    this.api.updateEquipmentItem(row.id, {
      categoryId: this.categoryId,
      name: this.name,
      machineType: this.machineType,
      summary: this.summary,
      typicalUse: this.typicalUse,
      description: this.description
    }).subscribe({
      next: async (saved) => {
        await notifySaved('Machine type updated', `${saved.name} is saved for the public Equipment page.`);
        this.reload();
      },
      error: () => {
        this.error.set('Could not update that machine type.');
        void notifyError('Could not update that machine type.');
      }
    });
  }

  move(row: EquipmentSummary, direction: -1 | 1): void {
    this.api.updateEquipmentItem(row.id, {
      categoryId: row.categoryId,
      name: row.name,
      sortOrder: row.sortOrder + direction
    }).subscribe({
      next: () => this.reload(),
      error: () => this.error.set('Could not change that machine order.')
    });
  }

  summaryOf(detail: EquipmentDetail): EquipmentSummary | undefined {
    return this.rows().find((row) => row.id === detail.id);
  }

  async remove(row: EquipmentSummary | EquipmentDetail): Promise<void> {
    const result = await confirmDelete(`Remove “${row.name}” from the machine type master?`);
    if (!result.isConfirmed) {
      return;
    }

    this.api.deleteEquipmentItem(row.id).subscribe({
      next: async () => {
        this.selected.set(null);
        await notifySaved('Machine type deleted', `${row.name} is no longer on the public list.`);
        this.reload();
      },
      error: () => {
        this.error.set('Could not delete that machine type.');
        void notifyError('Could not delete that machine type.');
      }
    });
  }

  private loadDetail(id: number): void {
    this.api.getEquipmentItem(id).subscribe({
      next: (row) => {
        this.selected.set(row);
        this.categoryId = row.category.id;
        this.name = row.name;
        this.machineType = row.machineType;
        this.summary = row.summary;
        this.typicalUse = row.typicalUse;
        this.description = row.description;
      },
      error: () => this.error.set('Could not load that machine type.')
    });
  }
}
