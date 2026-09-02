import { Injectable, signal } from '@angular/core';

const STORAGE_KEY = 'sararose.admin.session';

@Injectable({ providedIn: 'root' })
export class AuthService {
  readonly email = signal<string | null>(this.readEmail());
  readonly isSignedIn = signal(this.email() !== null);

  signIn(email: string, remember: boolean): void {
    const store = remember ? localStorage : sessionStorage;
    this.clearStores();
    store.setItem(STORAGE_KEY, email);
    this.email.set(email);
    this.isSignedIn.set(true);
  }

  signOut(): void {
    this.clearStores();
    this.email.set(null);
    this.isSignedIn.set(false);
  }

  private readEmail(): string | null {
    return sessionStorage.getItem(STORAGE_KEY) ?? localStorage.getItem(STORAGE_KEY);
  }

  private clearStores(): void {
    sessionStorage.removeItem(STORAGE_KEY);
    localStorage.removeItem(STORAGE_KEY);
  }
}
