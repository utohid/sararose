import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-header-manager',
  imports: [RouterLink],
  templateUrl: './header-manager.component.html',
  styleUrl: './header-manager.component.scss'
})
export class HeaderManagerComponent {
  readonly setdynamic = environment.setdynamic;
}
