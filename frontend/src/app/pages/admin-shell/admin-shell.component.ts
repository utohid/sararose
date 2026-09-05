import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-admin-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './admin-shell.component.html',
  styleUrl: './admin-shell.component.scss'
})
export class AdminShellComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly appName = 'SARA ROSE NIGERIA LIMITED';
  username = this.auth.username;
  email = this.auth.email;
  role = this.auth.role;
  userType = this.auth.userType;

  signOut(): void {
    this.auth.signOut();
    void this.router.navigateByUrl('/login');
  }
}
