import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { environment } from '../../../environments/environment';
import { ApiService, SliderSlide } from '../../services/api.service';

@Component({
  selector: 'app-slider-view',
  imports: [DatePipe, RouterLink],
  templateUrl: './slider-view.component.html',
  styleUrl: './slider-view.component.scss'
})
export class SliderViewComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);

  readonly setdynamic = environment.setdynamic;
  slides = signal<SliderSlide[]>([]);
  selected = signal<SliderSlide | null>(null);
  message = signal<string | null>(null);
  error = signal<string | null>(null);

  ngOnInit(): void {
    if (this.route.snapshot.queryParamMap.get('added') === '1') {
      this.message.set('Slide saved. It now appears in this list and on the home page while setdynamic is true.');
    }
    this.reload();
  }

  select(slide: SliderSlide): void {
    this.selected.set(slide);
  }

  reload(): void {
    this.api.getSlides().subscribe({
      next: (rows) => {
        this.slides.set(rows);
        const current = this.selected();
        const match = current ? rows.find((row) => row.id === current.id) : undefined;
        this.selected.set(match ?? rows[0] ?? null);
      },
      error: () => this.error.set('Could not load slider images. Confirm the API is running.')
    });
  }

  move(slide: SliderSlide, direction: -1 | 1): void {
    this.api.updateSlide(slide.id, { sortOrder: slide.sortOrder + direction }).subscribe({
      next: () => this.reload(),
      error: () => this.error.set('Could not change that slide order.')
    });
  }

  remove(slide: SliderSlide): void {
    this.api.deleteSlide(slide.id).subscribe({
      next: () => {
        this.message.set('Slide removed.');
        this.selected.set(null);
        this.reload();
      },
      error: () => this.error.set('Could not delete that image.')
    });
  }
}
