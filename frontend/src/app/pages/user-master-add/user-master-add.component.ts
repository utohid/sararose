import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { notifyError, notifySaved } from '../../notify';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-user-master-add',
  imports: [FormsModule, RouterLink],
  templateUrl: './user-master-add.component.html',
  styleUrl: './user-master-add.component.scss'
})
export class UserMasterAddComponent {
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);

  username = '';
  fullName = '';
  email = '';
  phone = '';
  password = '';
  role = 'User';
  userType = 'Customer';
  active = true;
  busy = signal(false);
  error = signal<string | null>(null);

  save(): void {
    if (!this.username.trim() || !this.fullName.trim() || !this.email.trim() || !this.phone.trim() || this.password.length < 8) {
      this.error.set('Enter username, full name, email, telephone and a password of at least 8 characters.');
      return;
    }

    this.busy.set(true);
    this.error.set(null);
    this.api.createUserMaster({
      username: this.username,
      fullName: this.fullName,
      email: this.email,
      phone: this.phone,
      password: this.password,
      role: this.role,
      userType: this.userType,
      active: this.active
    }).subscribe({
      next: async (row) => {
        this.busy.set(false);
        await notifySaved('UserMaster saved', `${row.username} can now sign in on the Login page.`);
        void this.router.navigate(['/dashboard/users']);
      },
      error: (err: HttpErrorResponse) => {
        this.busy.set(false);
        const message = typeof err.error?.message === 'string'
          ? err.error.message
          : 'Could not save that UserMaster record.';
        this.error.set(message);
        void notifyError(message);
      }
    });
  }
}
