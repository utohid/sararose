import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService, Category, Company } from '../../services/api.service';

@Component({
  selector: 'app-home',
  imports: [RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit, OnDestroy {
  private readonly api = inject(ApiService);
  private timer: ReturnType<typeof setInterval> | null = null;

  company = signal<Company | null>(null);
  categories = signal<Category[]>([]);
  error = signal<string | null>(null);
  active = signal(0);
  paused = signal(false);

  readonly slides = [
    { src: '/images/slide-excavator.jpg', alt: 'Excavator on a construction site at sunset' },
    { src: '/images/slide-forklift.jpg', alt: 'Forklift in an industrial yard' },
    { src: '/images/slide-bulldozer.jpg', alt: 'Bulldozer moving earth' },
    { src: '/images/slide-crane.jpg', alt: 'Crane and dump truck on a highway worksite' }
  ];

  ngOnInit(): void {
    this.api.getCompany().subscribe({
      next: (c) => this.company.set(c),
      error: () => this.error.set('Unable to load company details. Confirm the API is running.')
    });
    this.api.getCategories().subscribe({
      next: (rows) => this.categories.set(rows),
      error: () => this.error.set('Unable to load equipment categories.')
    });
    this.startSlider();
  }

  ngOnDestroy(): void {
    this.stopSlider();
  }

  goTo(index: number): void {
    this.active.set((index + this.slides.length) % this.slides.length);
  }

  private startSlider(): void {
    this.stopSlider();
    this.timer = setInterval(() => {
      if (!this.paused()) {
        this.goTo(this.active() + 1);
      }
    }, 2000);
  }

  private stopSlider(): void {
    if (this.timer) {
      clearInterval(this.timer);
      this.timer = null;
    }
  }
}
