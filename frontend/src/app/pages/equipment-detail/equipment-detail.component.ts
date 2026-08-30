import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiService, EquipmentDetail } from '../../services/api.service';

@Component({
  selector: 'app-equipment-detail',
  imports: [RouterLink],
  templateUrl: './equipment-detail.component.html',
  styleUrl: './equipment-detail.component.scss'
})
export class EquipmentDetailComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);

  item = signal<EquipmentDetail | null>(null);
  error = signal<string | null>(null);
  loading = signal(true);

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const slug = params.get('slug');
      if (!slug) {
        this.error.set('Missing equipment reference.');
        this.loading.set(false);
        return;
      }
      this.loading.set(true);
      this.api.getEquipmentBySlug(slug).subscribe({
        next: (row) => {
          this.item.set(row);
          this.loading.set(false);
        },
        error: () => {
          this.item.set(null);
          this.error.set('That machine type is not in the current portfolio list.');
          this.loading.set(false);
        }
      });
    });
  }
}
