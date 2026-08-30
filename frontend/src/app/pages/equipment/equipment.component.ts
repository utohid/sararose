import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiService, Category, EquipmentSummary } from '../../services/api.service';

@Component({
  selector: 'app-equipment',
  imports: [RouterLink],
  templateUrl: './equipment.component.html',
  styleUrl: './equipment.component.scss'
})
export class EquipmentComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);

  categories = signal<Category[]>([]);
  items = signal<EquipmentSummary[]>([]);
  selected = signal<string | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.api.getCategories().subscribe({
      next: (rows) => this.categories.set(rows),
      error: () => this.error.set('Categories could not be loaded.')
    });

    this.route.queryParamMap.subscribe((params) => {
      const category = params.get('category');
      this.selected.set(category);
      this.load(category);
    });
  }

  private load(category: string | null): void {
    this.loading.set(true);
    this.error.set(null);
    this.api.getEquipment(category).subscribe({
      next: (rows) => {
        this.items.set(rows);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Equipment list could not be loaded. Check that the API and MySQL are running.');
        this.loading.set(false);
      }
    });
  }
}
