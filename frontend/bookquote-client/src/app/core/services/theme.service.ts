import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly themeKey = 'bookquote_theme';

  getTheme(): 'light' | 'dark' {
    return (localStorage.getItem(this.themeKey) as 'light' | 'dark') || 'light';
  }

  setTheme(theme: 'light' | 'dark'): void {
    document.documentElement.setAttribute('data-bs-theme', theme);
    localStorage.setItem(this.themeKey, theme);
  }

  toggleTheme(): void {
    const currentTheme = this.getTheme();
    const newTheme = currentTheme === 'light' ? 'dark' : 'light';

    this.setTheme(newTheme);
  }

  initializeTheme(): void {
    this.setTheme(this.getTheme());
  }
}
