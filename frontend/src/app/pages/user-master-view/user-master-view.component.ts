import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { confirmDelete, notifyError, notifySaved } from '../../notify';
import { ApiService, UserMaster } from '../../services/api.service';

@Component({
  selector: 'app-user-master-view',
  imports: [FormsModule, RouterLink],
  templateUrl: './user-master-view.component.html',
  styleUrl: './user-master-view.component.scss'
})
export class UserMasterViewComponent implements OnInit {
  private readonly api = inject(ApiService);

  rows = signal<UserMaster[]>([]);
  selected = signal<UserMaster | null>(null);
  username = '';
  fullName = '';
  email = '';
  phone = '';
  password = '';
  role = 'User';
  userType = 'Customer';
  active = true;
  statusFilter = signal<'all' | 'active' | 'inactive'>('all');
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.reload();
  }

  select(row: UserMaster | null): void {
    this.selected.set(row);
    this.username = row?.username ?? '';
    this.fullName = row?.fullName ?? '';
    this.email = row?.email ?? '';
    this.phone = row?.phone ?? '';
    this.password = '';
    this.role = row?.role ?? 'User';
    this.userType = row?.userType ?? 'Customer';
    this.active = row?.active ?? true;
  }

  visibleRows(): UserMaster[] {
    const rows = this.rows();
    if (this.statusFilter() === 'active') {
      return rows.filter((row) => row.active);
    }
    if (this.statusFilter() === 'inactive') {
      return rows.filter((row) => !row.active);
    }
    return rows;
  }

  reload(): void {
    this.api.getUserMasters().subscribe({
      next: (rows) => {
        this.rows.set(rows);
        const current = this.selected();
        const match = current ? rows.find((row) => row.id === current.id) : undefined;
        this.select(match ?? rows[0] ?? null);
      },
      error: () => this.error.set('Could not load UserMaster. Confirm the API is running.')
    });
  }

  saveEdits(): void {
    const row = this.selected();
    if (!row || !this.username.trim() || !this.fullName.trim() || !this.email.trim() || !this.phone.trim()) {
      this.error.set('Enter username, full name, email and telephone.');
      return;
    }

    this.api.updateUserMaster(row.id, {
      username: this.username,
      fullName: this.fullName,
      email: this.email,
      phone: this.phone,
      password: this.password || undefined,
      role: this.role,
      userType: this.userType,
      active: this.active
    }).subscribe({
      next: async (saved) => {
        await notifySaved('UserMaster updated', `${saved.username} is saved on the UserMaster table.`);
        this.reload();
      },
      error: (err: HttpErrorResponse) => {
        const message = typeof err.error?.message === 'string'
          ? err.error.message
          : 'Could not update that UserMaster record.';
        this.error.set(message);
        void notifyError(message);
      }
    });
  }

  async remove(row: UserMaster): Promise<void> {
    const result = await confirmDelete(`Remove “${row.username}” from UserMaster? They will no longer be able to log in.`);
    if (!result.isConfirmed) {
      return;
    }

    this.api.deleteUserMaster(row.id).subscribe({
      next: async () => {
        this.selected.set(null);
        await notifySaved('UserMaster deleted', `${row.username} is no longer a login account.`);
        this.reload();
      },
      error: (err: HttpErrorResponse) => {
        const message = typeof err.error?.message === 'string'
          ? err.error.message
          : 'Could not delete that UserMaster record.';
        this.error.set(message);
        void notifyError(message);
      }
    });
  }
}
