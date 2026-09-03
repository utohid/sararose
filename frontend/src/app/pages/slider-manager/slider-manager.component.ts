import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { environment } from '../../../environments/environment';
import { ApiService, SliderSlide } from '../../services/api.service';

@Component({
  selector: 'app-slider-manager',
  imports: [FormsModule],
  templateUrl: './slider-manager.component.html',
  styleUrl: './slider-manager.component.scss'
})
export class SliderManagerComponent implements OnInit {
  private readonly api = inject(ApiService);

  readonly setdynamic = environment.setdynamic;
  slides = signal<SliderSlide[]>([]);
  altText = '';
  busy = signal(false);
  message = signal<string | null>(null);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    this.api.getSlides().subscribe({
      next: (rows) => this.slides.set(rows),
      error: () => this.error.set('Could not load slider images. Confirm the API is running.')
    });
  }

  onFile(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) {
      return;
    }

    this.busy.set(true);
    this.error.set(null);
    this.message.set(null);
    this.api.uploadSlide(file, this.altText).subscribe({
      next: () => {
        this.busy.set(false);
        this.altText = '';
        input.value = '';
        this.message.set('Image saved. The home page will use it while setdynamic is true.');
        this.reload();
      },
      error: () => {
        this.busy.set(false);
        this.error.set('Upload failed. Use a JPG, PNG, WEBP or GIF under 8 MB.');
      }
    });
  }

  move(slide: SliderSlide, direction: -1 | 1): void {
    const next = slide.sortOrder + direction;
    this.api.updateSlide(slide.id, { sortOrder: next }).subscribe({
      next: () => this.reload()
    });
  }

  remove(slide: SliderSlide): void {
    this.api.deleteSlide(slide.id).subscribe({
      next: () => this.reload(),
      error: () => this.error.set('Could not delete that image.')
    });
  }
}
