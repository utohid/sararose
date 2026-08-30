import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService, Company } from '../../services/api.service';

@Component({
  selector: 'app-why-us',
  imports: [RouterLink],
  templateUrl: './why-us.component.html',
  styleUrl: './why-us.component.scss'
})
export class WhyUsComponent implements OnInit {
  private readonly api = inject(ApiService);
  company = signal<Company | null>(null);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.api.getCompany().subscribe({
      next: (c) => this.company.set(c),
      error: () => this.error.set('Could not load reasons from the API.')
    });
  }
}
