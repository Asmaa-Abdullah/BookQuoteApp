import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Quote, CreateQuoteRequest, UpdateQuoteRequest } from '../models/quote.models';

import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class QuoteService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = `${environment.apiUrl}/Quotes`;

  getQuotes(): Observable<Quote[]> {
    return this.http.get<Quote[]>(this.apiUrl);
  }

  createQuote(request: CreateQuoteRequest): Observable<Quote> {
    return this.http.post<Quote>(this.apiUrl, request);
  }

  updateQuote(id: number, request: UpdateQuoteRequest): Observable<Quote> {
    return this.http.put<Quote>(`${this.apiUrl}/${id}`, request);
  }

  deleteQuote(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
