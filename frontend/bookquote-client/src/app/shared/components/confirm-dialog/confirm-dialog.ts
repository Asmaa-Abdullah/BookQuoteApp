import { Component, input, output } from '@angular/core';
import { AppButtonComponent } from '../app-button/app-button';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [AppButtonComponent],
  templateUrl: './confirm-dialog.html',
})
export class ConfirmDialogComponent {
  title = input('Confirm');
  message = input('Are you sure?');

  confirmed = output<void>();
  cancelled = output<void>();
}
