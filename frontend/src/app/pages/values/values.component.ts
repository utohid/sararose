import { Component, OnInit, inject, signal } from '@angular/core';
import { ApiService, Company } from '../../services/api.service';

@Component({
  selector: 'app-values',
  imports: [],
  templateUrl: './values.component.html',
  styleUrl: './values.component.scss'
})
export class ValuesComponent implements OnInit {
  private readonly api = inject(ApiService);
  company = signal<Company | null>(null);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.api.getCompany().subscribe({
      next: (c) => this.company.set(c),
      error: () => this.error.set('Vision and values could not be loaded.')
    });
  }
}
