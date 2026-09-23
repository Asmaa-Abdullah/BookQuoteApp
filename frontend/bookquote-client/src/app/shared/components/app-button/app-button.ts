import { Component, input } from '@angular/core';

@Component({
  selector: 'app-button',
  standalone: true,
  templateUrl: './app-button.html',
})
export class AppButtonComponent {
  label = input('');
  type = input('button');
  variant = input('primary');
  icon = input('');
  disabled = input(false);
}
