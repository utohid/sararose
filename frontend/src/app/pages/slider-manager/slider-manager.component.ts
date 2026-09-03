import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-slider-manager',
  imports: [RouterLink],
  templateUrl: './slider-manager.component.html',
  styleUrl: './slider-manager.component.scss'
})
export class SliderManagerComponent {
  readonly setdynamic = environment.setdynamic;
}
