import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ApiService, Registration } from '../../services/api.service';

@Component({
  selector: 'app-registrations-admin',
  imports: [DatePipe],
  templateUrl: './registrations-admin.component.html',
  styleUrl: './registrations-admin.component.scss'
})
export class RegistrationsAdminComponent implements OnInit {
  private readonly api = inject(ApiService);

  rows = signal<Registration[]>([]);
  selected = signal<Registration | null>(null);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.reload();
  }

  select(row: Registration): void {
    this.selected.set(row);
  }

  reload(): void {
    this.api.getRegistrations().subscribe({
      next: (rows) => {
        this.rows.set(rows);
        const current = this.selected();
        const match = current ? rows.find((row) => row.id === current.id) : undefined;
        this.selected.set(match ?? rows[0] ?? null);
      },
      error: () => this.error.set('Could not load registrations. Confirm the API is running.')
    });
  }
}
