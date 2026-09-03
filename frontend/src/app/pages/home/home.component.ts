import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { environment } from '../../../environments/environment';
import { ApiService, Category, Company } from '../../services/api.service';

export interface HomeSlide {
  src: string;
  alt: string;
}

const STATIC_SLIDES: HomeSlide[] = [
  { src: '/images/slide-excavator.jpg', alt: 'Excavator on a construction site at sunset' },
  { src: '/images/slide-forklift.jpg', alt: 'Forklift in an industrial yard' },
  { src: '/images/slide-bulldozer.jpg', alt: 'Bulldozer moving earth' },
  { src: '/images/slide-crane.jpg', alt: 'Crane and dump truck on a highway worksite' }
];

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
  slides = signal<HomeSlide[]>(STATIC_SLIDES);

  ngOnInit(): void {
    this.api.getCompany().subscribe({
      next: (c) => this.company.set(c),
      error: () => this.error.set('Unable to load company details. Confirm the API is running.')
    });
    this.api.getCategories().subscribe({
      next: (rows) => this.categories.set(rows),
      error: () => this.error.set('Unable to load equipment categories.')
    });

    if (environment.setdynamic === true) {
      this.api.getSlides().subscribe({
        next: (rows) => {
          if (rows.length) {
            this.slides.set(rows.map((row) => ({ src: row.url, alt: row.alt || 'SARA ROSE equipment' })));
            this.active.set(0);
          }
        }
      });
    }

    this.startSlider();
  }

  ngOnDestroy(): void {
    this.stopSlider();
  }

  goTo(index: number): void {
    const total = this.slides().length;
    if (!total) {
      return;
    }
    this.active.set((index + total) % total);
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
