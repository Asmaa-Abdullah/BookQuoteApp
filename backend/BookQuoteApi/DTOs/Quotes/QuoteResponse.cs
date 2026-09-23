namespace BookQuoteApi.DTOs.Quotes;

public class QuoteResponse
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }
}