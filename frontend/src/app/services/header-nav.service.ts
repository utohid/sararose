import { Injectable, computed, inject, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { ApiService, HeaderLink } from './api.service';

export const STATIC_HEADER_LINKS: HeaderLink[] = [
  { id: 1, label: 'Home', path: '/', sortOrder: 1, visible: true, isCta: false, createdAtUtc: '' },
  { id: 2, label: 'About', path: '/about', sortOrder: 2, visible: true, isCta: false, createdAtUtc: '' },
  { id: 3, label: 'Equipment', path: '/equipment', sortOrder: 3, visible: true, isCta: false, createdAtUtc: '' },
  { id: 4, label: 'Why us', path: '/why-sara-rose', sortOrder: 4, visible: true, isCta: false, createdAtUtc: '' },
  { id: 5, label: 'Vision', path: '/vision-values', sortOrder: 5, visible: true, isCta: false, createdAtUtc: '' },
  { id: 6, label: 'Registration', path: '/register', sortOrder: 6, visible: true, isCta: false, createdAtUtc: '' },
  { id: 7, label: 'Login', path: '/login', sortOrder: 7, visible: true, isCta: false, createdAtUtc: '' },
  { id: 8, label: 'Dashboard', path: '/dashboard', sortOrder: 8, visible: true, isCta: false, createdAtUtc: '' },
  { id: 9, label: 'Enquire', path: '/contact', sortOrder: 9, visible: true, isCta: true, createdAtUtc: '' }
];

@Injectable({ providedIn: 'root' })
export class HeaderNavService {
  private readonly api = inject(ApiService);

  readonly all = signal<HeaderLink[]>(STATIC_HEADER_LINKS);
  readonly visible = computed(() => this.all().filter((row) => row.visible));

  constructor() {
    this.reload();
  }

  reload(): void {
    if (!environment.setdynamic) {
      this.all.set(STATIC_HEADER_LINKS);
      return;
    }

    this.api.getHeaderLinks().subscribe({
      next: (rows) => this.all.set(rows.length ? rows : STATIC_HEADER_LINKS),
      error: () => this.all.set(STATIC_HEADER_LINKS)
    });
  }
}

export function isExternalPath(path: string): boolean {
  return /^https?:\/\//i.test(path);
}
