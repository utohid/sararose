import { Injectable, computed, signal } from '@angular/core';

const STORAGE_KEY = 'sararose.admin.session';
const TOKEN_KEY = 'sararose.admin.token';

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

export interface LoginSession {
  token: string;
  expiresAtUtc: string;
  user: AuthUser;
}

export type AppRole = 'Admin' | 'Staff' | 'User';

@Injectable({ providedIn: 'root' })
export class AuthService {
  readonly user = signal<AuthUser | null>(null);
  readonly token = signal<string | null>(null);
  readonly username = computed(() => this.user()?.username ?? null);
  readonly email = computed(() => this.user()?.email ?? null);
  readonly role = computed(() => this.user()?.role ?? null);
  readonly userType = computed(() => this.user()?.userType ?? null);
  readonly isSignedIn = computed(() => this.user() !== null && this.hasLiveToken());
  readonly isAdmin = computed(() => this.role() === 'Admin');
  readonly isStaff = computed(() => this.role() === 'Staff' || this.role() === 'Admin');

  constructor() {
    this.restore();
  }

  signIn(session: LoginSession, remember: boolean): void {
    const store = remember ? localStorage : sessionStorage;
    this.clearStores();
    store.setItem(STORAGE_KEY, JSON.stringify(session.user));
    store.setItem(TOKEN_KEY, JSON.stringify({
      token: session.token,
      expiresAtUtc: session.expiresAtUtc
    }));
    this.user.set(this.mergeClaims(session.user, session.token));
    this.token.set(session.token);
  }

  signOut(): void {
    this.clearStores();
    this.user.set(null);
    this.token.set(null);
  }

  canAccess(roles?: string[] | null): boolean {
    if (!this.isSignedIn()) {
      return false;
    }
    if (!roles || roles.length === 0) {
      return true;
    }
    const current = this.role();
    return current ? roles.includes(current) : false;
  }

  private restore(): void {
    const rawUser = sessionStorage.getItem(STORAGE_KEY) ?? localStorage.getItem(STORAGE_KEY);
    const rawToken = sessionStorage.getItem(TOKEN_KEY) ?? localStorage.getItem(TOKEN_KEY);
    if (!rawUser || !rawToken) {
      this.signOut();
      return;
    }

    try {
      const user = JSON.parse(rawUser) as AuthUser;
      const packed = JSON.parse(rawToken) as { token: string; expiresAtUtc: string };
      if (!packed.token || this.tokenExpired(packed.expiresAtUtc, packed.token)) {
        this.signOut();
        return;
      }
      this.token.set(packed.token);
      this.user.set(this.mergeClaims(user, packed.token));
    } catch {
      this.signOut();
    }
  }

  private hasLiveToken(): boolean {
    const token = this.token();
    if (!token) {
      return false;
    }
    const raw = sessionStorage.getItem(TOKEN_KEY) ?? localStorage.getItem(TOKEN_KEY);
    if (!raw) {
      return true;
    }
    try {
      const packed = JSON.parse(raw) as { expiresAtUtc: string };
      return !this.tokenExpired(packed.expiresAtUtc, token);
    } catch {
      return false;
    }
  }

  private tokenExpired(expiresAtUtc: string, token: string): boolean {
    const fromPayload = Number(this.readClaims(token)?.['exp']);
    const expires = Number.isFinite(fromPayload)
      ? fromPayload * 1000
      : Date.parse(expiresAtUtc);
    return !Number.isFinite(expires) || expires <= Date.now();
  }

  private mergeClaims(user: AuthUser, token: string): AuthUser {
    const claims = this.readClaims(token);
    if (!claims) {
      return user;
    }
    return {
      ...user,
      id: Number(claims['userId'] ?? claims['sub'] ?? user.id) || user.id,
      username: String(claims['name'] ?? claims['unique_name'] ?? user.username),
      fullName: String(claims['fullName'] ?? user.fullName),
      email: String(claims['email'] ?? user.email),
      phone: String(claims['phone'] ?? user.phone),
      company: (claims['company'] as string | undefined) || user.company,
      city: (claims['city'] as string | undefined) || user.city,
      role: String(claims['role'] ?? user.role),
      userType: String(claims['userType'] ?? user.userType)
    };
  }

  private readClaims(token: string): Record<string, unknown> | null {
    const parts = token.split('.');
    if (parts.length < 2) {
      return null;
    }
    try {
      const payload = parts[1].replace(/-/g, '+').replace(/_/g, '/');
      const padded = payload + '='.repeat((4 - (payload.length % 4)) % 4);
      return JSON.parse(atob(padded)) as Record<string, unknown>;
    } catch {
      return null;
    }
  }

  private clearStores(): void {
    sessionStorage.removeItem(STORAGE_KEY);
    sessionStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(STORAGE_KEY);
    localStorage.removeItem(TOKEN_KEY);
  }
}
