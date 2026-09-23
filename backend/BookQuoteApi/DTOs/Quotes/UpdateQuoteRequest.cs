namespace BookQuoteApi.DTOs.Quotes;

using System.ComponentModel.DataAnnotations;

public class UpdateQuoteRequest
{
    [Required]
    [MaxLength(1000)]
    public string Text { get; set; } = string.Empty;

    public bool IsFavorite { get; set; }
}