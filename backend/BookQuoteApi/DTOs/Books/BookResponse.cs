namespace BookQuoteApi.DTOs.Books;

public class BookResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public DateTime PublicationDate { get; set; }
}