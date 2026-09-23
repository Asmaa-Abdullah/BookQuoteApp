using BookQuoteApi.DTOs.Quotes;

namespace BookQuoteApi.Services.Interfaces;

public interface IQuoteService
{
    Task<List<QuoteResponse>> GetMyQuotesAsync(int userId);
    Task<QuoteResponse> CreateAsync(int userId, CreateQuoteRequest request);
    Task<QuoteResponse?> UpdateAsync(
        int userId,
        int id,
        UpdateQuoteRequest request);
    Task<bool> DeleteAsync(int userId, int id);
}