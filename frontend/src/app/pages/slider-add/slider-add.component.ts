import { DecimalPipe } from '@angular/common';
import { Component, OnDestroy, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { environment } from '../../../environments/environment';
import { notifyError, notifySaved } from '../../notify';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-slider-add',
  imports: [DecimalPipe, FormsModule, RouterLink],
  templateUrl: './slider-add.component.html',
  styleUrl: './slider-add.component.scss'
})
export class SliderAddComponent implements OnDestroy {
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);

  readonly setdynamic = environment.setdynamic;
  altText = '';
  file: File | null = null;
  previewUrl = signal<string | null>(null);
  busy = signal(false);
  error = signal<string | null>(null);

  ngOnDestroy(): void {
    this.revokePreview();
  }

  onFile(event: Event): void {
    const input = event.target as HTMLInputElement;
    const next = input.files?.[0] ?? null;
    this.file = next;
    this.error.set(null);
    this.revokePreview();
    if (next) {
      this.previewUrl.set(URL.createObjectURL(next));
    }
  }

  save(): void {
    if (!this.file) {
      this.error.set('Choose an image before saving.');
      return;
    }

    this.busy.set(true);
    this.error.set(null);
    this.api.uploadSlide(this.file, this.altText).subscribe({
      next: async () => {
        this.busy.set(false);
        await notifySaved('Slide saved', 'The image is on the view page and on home while setdynamic is true.');
        void this.router.navigate(['/dashboard/slider/view']);
      },
      error: () => {
        this.busy.set(false);
        this.error.set('Upload failed. Use a JPG, PNG, WEBP or GIF under 8 MB.');
        void notifyError('Could not save that slide.');
      }
    });
  }

  private revokePreview(): void {
    const url = this.previewUrl();
    if (url) {
      URL.revokeObjectURL(url);
      this.previewUrl.set(null);
    }
  }
}
