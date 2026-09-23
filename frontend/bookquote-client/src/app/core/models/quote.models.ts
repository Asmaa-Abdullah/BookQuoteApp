export interface Quote {
  id: number;
  text: string;
  isFavorite: boolean;
}

export interface CreateQuoteRequest {
  text: string;
}

export interface UpdateQuoteRequest {
  text: string;
}
