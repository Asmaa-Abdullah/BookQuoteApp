using BookQuoteApi.Data;
using BookQuoteApi.DTOs.Quotes;
using BookQuoteApi.Models;
using BookQuoteApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookQuoteApi.Services;

public class QuoteService : IQuoteService
{
    private readonly AppDbContext _context;

    public QuoteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<QuoteResponse>> GetMyQuotesAsync(int userId)
    {
        return await _context.Quotes
            .AsNoTracking()
            .Where(q => q.UserId == userId)
            .Select(q => new QuoteResponse
            {
                Id = q.Id,
                Text = q.Text,
                IsFavorite = q.IsFavorite
            })
            .ToListAsync();
    }

    public async Task<QuoteResponse> CreateAsync(
        int userId,
        CreateQuoteRequest request)
    {
        var quote = new Quote
        {
            Text = request.Text,
            IsFavorite = request.IsFavorite,
            UserId = userId
        };

        _context.Quotes.Add(quote);
        await _context.SaveChangesAsync();

        return new QuoteResponse
        {
            Id = quote.Id,
            Text = quote.Text,
            IsFavorite = quote.IsFavorite
        };
    }

    public async Task<QuoteResponse?> UpdateAsync(
        int userId,
        int id,
        UpdateQuoteRequest request)
    {
        var quote = await _context.Quotes
            .SingleOrDefaultAsync(q =>
                q.Id == id &&
                q.UserId == userId);

        if (quote is null)
        {
            return null;
        }

        quote.Text = request.Text;
        quote.IsFavorite = request.IsFavorite;

        await _context.SaveChangesAsync();

        return new QuoteResponse
        {
            Id = quote.Id,
            Text = quote.Text,
            IsFavorite = quote.IsFavorite
        };
    }

    public async Task<bool> DeleteAsync(int userId, int id)
    {
        var quote = await _context.Quotes
            .SingleOrDefaultAsync(q =>
                q.Id == id &&
                q.UserId == userId);

        if (quote is null)
        {
            return false;
        }

        _context.Quotes.Remove(quote);
        await _context.SaveChangesAsync();

        return true;
    }
}