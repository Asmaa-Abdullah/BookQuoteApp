namespace BookQuoteApi.DTOs.Books;

using System.ComponentModel.DataAnnotations;

public class CreateBookRequest
{
   [Required]
   [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Author { get; set; } = string.Empty;

    [Required]
    public DateTime PublicationDate { get; set; }
}