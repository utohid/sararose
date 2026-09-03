import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { HeaderNavService, isExternalPath } from './services/header-nav.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  private readonly headerNav = inject(HeaderNavService);

  readonly year = new Date().getFullYear();
  readonly contactEmail = 'contact@sararose.com';
  readonly links = this.headerNav.visible;
  readonly isExternal = isExternalPath;
  menuOpen = false;

  toggleMenu(): void {
    this.menuOpen = !this.menuOpen;
  }

  closeMenu(): void {
    this.menuOpen = false;
  }
}
