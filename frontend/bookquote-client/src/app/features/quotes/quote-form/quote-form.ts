import { AfterViewInit, Component, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { QuoteService } from '../../../core/services/quote.service';

import { AppButtonComponent } from '../../../shared/components/app-button/app-button';
import { AppCardComponent } from '../../../shared/components/app-card/app-card';

@Component({
  selector: 'app-quote-form',
  imports: [ReactiveFormsModule, RouterLink, AppButtonComponent, AppCardComponent],
  templateUrl: './quote-form.html',
})
export class QuoteForm implements OnInit, AfterViewInit {
  private readonly quoteService = inject(QuoteService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  @ViewChild('quoteText')
  quoteText!: ElementRef<HTMLTextAreaElement>;

  quoteId: number | null = null;
  isEditMode = false;

  loading = false;
  saving = false;

  quoteForm = new FormGroup({
    text: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(500)],
    }),
    isFavorite: new FormControl(false, {
      nonNullable: true,
    }),
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.quoteId = Number(id);
      this.isEditMode = true;
      this.loadQuote(this.quoteId);
    }
  }

  ngAfterViewInit(): void {
    if (!this.isEditMode) {
      this.quoteText.nativeElement.focus();
    }
  }

  private loadQuote(id: number): void {
    this.loading = true;

    this.quoteService.getQuotes().subscribe({
      next: (quotes) => {
        const quote = quotes.find((item) => item.id === id);

        if (!quote) {
          this.router.navigate(['/quotes']);
          return;
        }

        this.quoteForm.patchValue({
          text: quote.text,
          isFavorite: quote.isFavorite,
        });

        this.loading = false;
      },
      error: (error) => {
        console.error('Failed to load quote:', error);
        this.loading = false;
        this.router.navigate(['/quotes']);
      },
    });
  }

  onSubmit(): void {
    if (this.quoteForm.invalid) {
      this.quoteForm.markAllAsTouched();
      return;
    }

    this.saving = true;

    const request = this.quoteForm.getRawValue();

    if (this.isEditMode && this.quoteId !== null) {
      this.quoteService.updateQuote(this.quoteId, request).subscribe({
        next: () => {
          this.router.navigate(['/quotes']);
        },
        error: (error) => {
          console.error('Failed to update quote:', error);
          this.saving = false;
        },
      });

      return;
    }

    this.quoteService.createQuote(request).subscribe({
      next: () => {
        this.router.navigate(['/quotes']);
      },
      error: (error) => {
        console.error('Failed to create quote:', error);
        this.saving = false;
      },
    });
  }
}
