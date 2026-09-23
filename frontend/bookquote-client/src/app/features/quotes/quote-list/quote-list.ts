import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { Quote } from '../../../core/models/quote.models';
import { QuoteService } from '../../../core/services/quote.service';

import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-quote-list',
  imports: [RouterLink, ConfirmDialogComponent],
  templateUrl: './quote-list.html',
  styleUrl: './quote-list.scss',
})
export class QuoteList implements OnInit {
  private readonly quoteService = inject(QuoteService);
  private readonly router = inject(Router);

  quotes: Quote[] = [];
  loading = false;

  get favoriteQuotes(): Quote[] {
    return this.quotes.filter((quote) => quote.isFavorite).slice(0, 5);
  }

  showDeleteDialog = false;
  quoteToDelete: Quote | null = null;

  ngOnInit(): void {
    this.loadQuotes();
  }

  loadQuotes(): void {
    this.loading = true;

    this.quoteService.getQuotes().subscribe({
      next: (quotes) => {
        this.quotes = quotes;
        this.loading = false;
      },
      error: (error) => {
        console.error('Failed to load quotes:', error);
        this.loading = false;
      },
    });
  }

  editQuote(quoteId: number): void {
    this.router.navigate(['/quotes/edit', quoteId]);
  }

  deleteQuote(quote: Quote): void {
    this.quoteToDelete = quote;
    this.showDeleteDialog = true;
  }

  confirmDelete(): void {
    if (!this.quoteToDelete) {
      return;
    }

    const quoteId = this.quoteToDelete.id;

    this.quoteService.deleteQuote(quoteId).subscribe({
      next: () => {
        this.quotes = this.quotes.filter((quote) => quote.id !== quoteId);

        this.closeDeleteDialog();
      },
      error: (error) => {
        console.error('Failed to delete quote:', error);
      },
    });
  }

  closeDeleteDialog(): void {
    this.showDeleteDialog = false;
    this.quoteToDelete = null;
  }
}
