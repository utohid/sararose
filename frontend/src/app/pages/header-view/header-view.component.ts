import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { environment } from '../../../environments/environment';
import { confirmDelete, notifyError, notifySaved } from '../../notify';
import { ApiService, HeaderLink } from '../../services/api.service';
import { HeaderNavService } from '../../services/header-nav.service';

@Component({
  selector: 'app-header-view',
  imports: [DatePipe, FormsModule, RouterLink],
  templateUrl: './header-view.component.html',
  styleUrl: './header-view.component.scss'
})
export class HeaderViewComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly nav = inject(HeaderNavService);

  readonly setdynamic = environment.setdynamic;
  links = signal<HeaderLink[]>([]);
  selected = signal<HeaderLink | null>(null);
  label = '';
  path = '';
  visible = true;
  isCta = false;
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.reload();
  }

  select(link: HeaderLink | null): void {
    this.selected.set(link);
    this.label = link?.label ?? '';
    this.path = link?.path ?? '';
    this.visible = link?.visible ?? true;
    this.isCta = link?.isCta ?? false;
  }

  reload(): void {
    this.api.getHeaderLinks().subscribe({
      next: (rows) => {
        this.links.set(rows);
        const current = this.selected();
        const match = current ? rows.find((row) => row.id === current.id) : undefined;
        this.select(match ?? rows[0] ?? null);
      },
      error: () => this.error.set('Could not load header links. Confirm the API is running.')
    });
  }

  saveEdits(): void {
    const link = this.selected();
    if (!link) {
      return;
    }

    this.api.updateHeaderLink(link.id, {
      label: this.label,
      path: this.path,
      visible: this.visible,
      isCta: this.isCta
    }).subscribe({
      next: async () => {
        this.nav.reload();
        await notifySaved('Link updated', `${this.label.trim()} is saved for the site header.`);
        this.reload();
      },
      error: () => {
        this.error.set('Could not update that link.');
        void notifyError('Could not update that header link.');
      }
    });
  }

  move(link: HeaderLink, direction: -1 | 1): void {
    this.api.updateHeaderLink(link.id, { sortOrder: link.sortOrder + direction }).subscribe({
      next: () => {
        this.nav.reload();
        this.reload();
      },
      error: () => this.error.set('Could not change that link order.')
    });
  }

  async remove(link: HeaderLink): Promise<void> {
    const result = await confirmDelete(`Remove “${link.label}” from the header?`);
    if (!result.isConfirmed) {
      return;
    }

    this.api.deleteHeaderLink(link.id).subscribe({
      next: async () => {
        this.selected.set(null);
        this.nav.reload();
        await notifySaved('Link deleted', `${link.label} is no longer in the header.`);
        this.reload();
      },
      error: () => {
        this.error.set('Could not delete that link.');
        void notifyError('Could not delete that header link.');
      }
    });
  }
}
