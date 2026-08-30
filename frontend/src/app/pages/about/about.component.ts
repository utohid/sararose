import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService, Company } from '../../services/api.service';

@Component({
  selector: 'app-about',
  imports: [RouterLink],
  templateUrl: './about.component.html',
  styleUrl: './about.component.scss'
})
export class AboutComponent implements OnInit {
  private readonly api = inject(ApiService);
  company = signal<Company | null>(null);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.api.getCompany().subscribe({
      next: (c) => this.company.set(c),
      error: () => this.error.set('Company details could not be loaded.')
    });
  }
}
