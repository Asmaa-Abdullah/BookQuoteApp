import { AfterViewInit, Component, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';

import { Book } from '../../../core/models/book.models';
import { BookService } from '../../../core/services/book.service';

import { AppButtonComponent } from '../../../shared/components/app-button/app-button';
import { AppCardComponent } from '../../../shared/components/app-card/app-card';
import { AppTableComponent } from '../../../shared/components/app-table/app-table';

@Component({
  selector: 'app-book-form',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    DatePipe,
    AppButtonComponent,
    AppCardComponent,
    AppTableComponent,
  ],
  templateUrl: './book-form.html',
})
export class BookForm implements OnInit, AfterViewInit {
  private readonly bookService = inject(BookService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  @ViewChild('bookTitle')
  bookTitle!: ElementRef<HTMLInputElement>;

  bookId: number | null = null;
  isEditMode = false;

  loading = false;
  saving = false;

  addedBooks: Book[] = [];

  bookForm = new FormGroup({
    title: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    author: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    publicationDate: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.bookId = Number(id);
      this.isEditMode = true;
      this.loadBook(this.bookId);
    }
  }

  ngAfterViewInit(): void {
    if (!this.isEditMode) {
      this.bookTitle.nativeElement.focus();
    }
  }

  private loadBook(id: number): void {
    this.loading = true;

    this.bookService.getBook(id).subscribe({
      next: (book) => {
        this.bookForm.patchValue({
          title: book.title,
          author: book.author,
          publicationDate: book.publicationDate.substring(0, 10),
        });

        this.loading = false;
      },
      error: (error) => {
        console.error('Failed to load book:', error);
        this.loading = false;
        this.router.navigate(['/books']);
      },
    });
  }

  onSubmit(): void {
    if (this.bookForm.invalid) {
      this.bookForm.markAllAsTouched();
      return;
    }

    this.saving = true;

    const request = this.bookForm.getRawValue();

    if (this.isEditMode && this.bookId !== null) {
      this.bookService.updateBook(this.bookId, request).subscribe({
        next: () => {
          this.router.navigate(['/books']);
        },
        error: (error) => {
          console.error('Failed to update book:', error);
          this.saving = false;
        },
      });

      return;
    }

    this.bookService.createBook(request).subscribe({
      next: (book) => {
        this.addedBooks = [book, ...this.addedBooks];

        this.bookForm.reset({
          title: '',
          author: '',
          publicationDate: '',
        });

        this.saving = false;
      },
      error: (error) => {
        console.error('Failed to create book:', error);
        this.saving = false;
      },
    });
  }
}
