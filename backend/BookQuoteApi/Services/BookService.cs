using BookQuoteApi.Data;
using BookQuoteApi.DTOs.Books;
using BookQuoteApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookQuoteApi.Services;

public class BookService : IBookService
{
    private readonly AppDbContext _context;

    public BookService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BookResponse>> GetAllAsync()
    {
        return await _context.Books
            .AsNoTracking()
            .Select(book => new BookResponse
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PublicationDate = book.PublicationDate
            })
            .ToListAsync();
    }

    public async Task<BookResponse?> GetByIdAsync(int id)
    {
        return await _context.Books
            .AsNoTracking()
            .Where(book => book.Id == id)
            .Select(book => new BookResponse
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PublicationDate = book.PublicationDate
            })
            .SingleOrDefaultAsync();
    }

    public async Task<BookResponse> CreateAsync(CreateBookRequest request)
    {
        var book = new Models.Book
        {
            Title = request.Title,
            Author = request.Author,
            PublicationDate = request.PublicationDate
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            PublicationDate = book.PublicationDate
        };
    }

    public async Task<BookResponse?> UpdateAsync(
        int id,
        UpdateBookRequest request)
    {
        var book = await _context.Books
            .SingleOrDefaultAsync(b => b.Id == id);

        if (book is null)
        {
            return null;
        }

        book.Title = request.Title;
        book.Author = request.Author;
        book.PublicationDate = request.PublicationDate;

        await _context.SaveChangesAsync();

        return new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            PublicationDate = book.PublicationDate
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var book = await _context.Books
            .SingleOrDefaultAsync(b => b.Id == id);

        if (book is null)
        {
            return false;
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return true;
    }
}