import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Book, CreateBookRequest, UpdateBookRequest } from '../models/book.models';

import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class BookService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = `${environment.apiUrl}/Books`;

  getBooks(): Observable<Book[]> {
    return this.http.get<Book[]>(this.apiUrl);
  }

  getBook(id: number): Observable<Book> {
    return this.http.get<Book>(`${this.apiUrl}/${id}`);
  }

  createBook(request: CreateBookRequest): Observable<Book> {
    return this.http.post<Book>(this.apiUrl, request);
  }

  updateBook(id: number, request: UpdateBookRequest): Observable<Book> {
    return this.http.put<Book>(`${this.apiUrl}/${id}`, request);
  }

  deleteBook(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
