using BookQuoteApi.DTOs.Books;

namespace BookQuoteApi.Services.Interfaces;

public interface IBookService
{
    Task<List<BookResponse>> GetAllAsync();

    Task<BookResponse?> GetByIdAsync(int id);

    Task<BookResponse> CreateAsync(CreateBookRequest request);

    Task<BookResponse?> UpdateAsync(int id, UpdateBookRequest request);

    Task<bool> DeleteAsync(int id);
}