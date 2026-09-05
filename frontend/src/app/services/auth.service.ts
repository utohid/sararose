import { Injectable, computed, signal } from '@angular/core';

const STORAGE_KEY = 'sararose.admin.session';

export interface AuthUser {
  id: number;
  username: string;
  fullName: string;
  email: string;
  phone: string;
  company?: string | null;
  city?: string | null;
  role: string;
  userType: string;
  createdAtUtc: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  readonly user = signal<AuthUser | null>(this.readUser());
  readonly username = computed(() => this.user()?.username ?? null);
  readonly email = computed(() => this.user()?.email ?? null);
  readonly role = computed(() => this.user()?.role ?? null);
  readonly userType = computed(() => this.user()?.userType ?? null);
  readonly isSignedIn = computed(() => this.user() !== null);

  signIn(user: AuthUser, remember: boolean): void {
    const store = remember ? localStorage : sessionStorage;
    this.clearStores();
    store.setItem(STORAGE_KEY, JSON.stringify(user));
    this.user.set(user);
  }

  signOut(): void {
    this.clearStores();
    this.user.set(null);
  }

  private readUser(): AuthUser | null {
    const raw = sessionStorage.getItem(STORAGE_KEY) ?? localStorage.getItem(STORAGE_KEY);
    if (!raw) {
      return null;
    }

    try {
      const parsed = JSON.parse(raw) as AuthUser;
      return parsed?.username || parsed?.email ? parsed : null;
    } catch {
      return null;
    }
  }

  private clearStores(): void {
    sessionStorage.removeItem(STORAGE_KEY);
    localStorage.removeItem(STORAGE_KEY);
  }
}
