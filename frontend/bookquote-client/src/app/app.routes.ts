import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
  },

  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register').then((m) => m.Register),
  },

  {
    path: 'books',
    canActivate: [authGuard],
    children: [
      {
        path: '',
        loadComponent: () => import('./features/books/book-list/book-list').then((m) => m.BookList),
      },

      {
        path: 'add',
        loadComponent: () => import('./features/books/book-form/book-form').then((m) => m.BookForm),
      },

      {
        path: 'edit/:id',
        loadComponent: () => import('./features/books/book-form/book-form').then((m) => m.BookForm),
      },
    ],
  },

  {
    path: 'quotes',
    canActivate: [authGuard],
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/quotes/quote-list/quote-list').then((m) => m.QuoteList),
      },

      {
        path: 'add',
        loadComponent: () =>
          import('./features/quotes/quote-form/quote-form').then((m) => m.QuoteForm),
      },

      {
        path: 'all',
        loadComponent: () =>
          import('./features/quotes/all-quotes/all-quotes').then((m) => m.AllQuotes),
      },

      {
        path: 'edit/:id',
        loadComponent: () =>
          import('./features/quotes/quote-form/quote-form').then((m) => m.QuoteForm),
      },
    ],
  },

  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },

  {
    path: '**',
    redirectTo: 'books',
    pathMatch: 'full',
  },
];
