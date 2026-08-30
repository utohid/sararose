import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService, Category, Company } from '../../services/api.service';

@Component({
  selector: 'app-home',
  imports: [RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  private readonly api = inject(ApiService);
  company = signal<Company | null>(null);
  categories = signal<Category[]>([]);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.api.getCompany().subscribe({
      next: (c) => this.company.set(c),
      error: () => this.error.set('Unable to load company details. Confirm the API is running.')
    });
    this.api.getCategories().subscribe({
      next: (rows) => this.categories.set(rows),
      error: () => this.error.set('Unable to load equipment categories.')
    });
  }
}
