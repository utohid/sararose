import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { environment } from '../../../environments/environment';
import { notifyError, notifySaved } from '../../notify';
import { ApiService } from '../../services/api.service';
import { HeaderNavService } from '../../services/header-nav.service';

@Component({
  selector: 'app-header-add',
  imports: [FormsModule, RouterLink],
  templateUrl: './header-add.component.html',
  styleUrl: './header-add.component.scss'
})
export class HeaderAddComponent {
  private readonly api = inject(ApiService);
  private readonly nav = inject(HeaderNavService);
  private readonly router = inject(Router);

  readonly setdynamic = environment.setdynamic;
  label = '';
  path = '';
  visible = true;
  isCta = false;
  busy = signal(false);
  error = signal<string | null>(null);

  save(): void {
    if (!this.label.trim() || !this.path.trim()) {
      this.error.set('Enter a label and a path, for example About and /about.');
      return;
    }

    this.busy.set(true);
    this.error.set(null);
    this.api.createHeaderLink({
      label: this.label,
      path: this.path,
      visible: this.visible,
      isCta: this.isCta
    }).subscribe({
      next: async () => {
        this.nav.reload();
        this.busy.set(false);
        await notifySaved('Link saved', `${this.label.trim()} is now in the header list.`);
        void this.router.navigate(['/dashboard/header/view']);
      },
      error: () => {
        this.busy.set(false);
        this.error.set('Could not save that link. Use a label and a path starting with / or http.');
        void notifyError('Could not save that header link.');
      }
    });
  }
}
