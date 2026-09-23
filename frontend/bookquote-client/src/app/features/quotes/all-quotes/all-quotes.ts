import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';

import { Quote } from '../../../core/models/quote.models';
import { QuoteService } from '../../../core/services/quote.service';

import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-all-quotes',
  imports: [RouterLink, ConfirmDialogComponent],
  templateUrl: './all-quotes.html',
  styleUrl: './all-quotes.scss',
})
export class AllQuotes implements OnInit {
  private readonly quoteService = inject(QuoteService);

  quotes: Quote[] = [];
  loading = false;

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

  toggleFavorite(quote: Quote): void {
    const updatedQuote = {
      text: quote.text,
      isFavorite: !quote.isFavorite,
    };

    this.quoteService.updateQuote(quote.id, updatedQuote).subscribe({
      next: (updated) => {
        quote.isFavorite = updated.isFavorite;
      },
      error: (error) => {
        console.error('Failed to update favorite:', error);
      },
    });
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
