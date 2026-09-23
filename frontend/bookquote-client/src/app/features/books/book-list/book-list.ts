import { DatePipe } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { Book } from '../../../core/models/book.models';
import { BookService } from '../../../core/services/book.service';

import { AppButtonComponent } from '../../../shared/components/app-button/app-button';
import { AppCardComponent } from '../../../shared/components/app-card/app-card';
import { AppTableComponent } from '../../../shared/components/app-table/app-table';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-book-list',
  imports: [
    FormsModule,
    RouterLink,
    DatePipe,
    AppCardComponent,
    AppTableComponent,
    EmptyStateComponent,
    ConfirmDialogComponent,
  ],
  templateUrl: './book-list.html',
})
export class BookList implements OnInit {
  private readonly bookService = inject(BookService);
  private readonly router = inject(Router);

  books: Book[] = [];
  filteredBooks: Book[] = [];

  searchTerm = '';
  loading = false;

  showDeleteDialog = false;
  bookToDelete: Book | null = null;

  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks(): void {
    this.loading = true;

    this.bookService.getBooks().subscribe({
      next: (books) => {
        this.books = books;
        this.filteredBooks = books;
        this.loading = false;
      },
      error: (error) => {
        console.error('Failed to load books:', error);
        this.loading = false;
      },
    });
  }

  searchBooks(): void {
    const search = this.searchTerm.trim().toLowerCase();

    if (!search) {
      this.filteredBooks = this.books;
      return;
    }

    this.filteredBooks = this.books.filter((book) => book.title.toLowerCase().includes(search));
  }

  editBook(bookId: number): void {
    console.log('EDIT CLICKED:', bookId);

    this.router.navigate(['/books/edit', bookId]);
  }

  deleteBook(book: Book): void {
    this.bookToDelete = book;
    this.showDeleteDialog = true;
  }

  confirmDelete(): void {
    if (!this.bookToDelete) {
      return;
    }

    const bookId = this.bookToDelete.id;

    this.bookService.deleteBook(bookId).subscribe({
      next: () => {
        this.books = this.books.filter((book) => book.id !== bookId);
        this.filteredBooks = this.filteredBooks.filter((book) => book.id !== bookId);
        this.closeDeleteDialog();
      },
      error: (error) => {
        console.error('Failed to delete book:', error);
      },
    });
  }

  closeDeleteDialog(): void {
    this.showDeleteDialog = false;
    this.bookToDelete = null;
  }
}
