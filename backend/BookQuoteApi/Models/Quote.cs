namespace BookQuoteApi.Models;

public class Quote
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public bool IsFavorite { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;
}