import { Component, input } from '@angular/core';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  templateUrl: './empty-state.html',
})
export class EmptyStateComponent {
  message = input('No data available.');
  icon = input('fa-solid fa-inbox');
}
