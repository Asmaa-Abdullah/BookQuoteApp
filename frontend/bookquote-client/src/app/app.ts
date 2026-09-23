import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { Navbar } from './layout/navbar/navbar';
import { ThemeService } from './core/services/theme.service';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Navbar],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  private readonly themeService = inject(ThemeService);
  protected readonly authService = inject(AuthService);

  constructor() {
    this.themeService.initializeTheme();
  }
}
