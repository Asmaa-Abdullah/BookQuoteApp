using BookQuoteApi.Data;
using BookQuoteApi.DTOs.Books;
using BookQuoteApi.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookQuoteApi.Tests;

public class BookServiceTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static BookService CreateBookService(
        AppDbContext context)
    {
        return new BookService(context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllBooks()
    {
        // Arrange
        await using var context = CreateDbContext();

        context.Books.AddRange(
            new Models.Book
            {
                Title = "Book One",
                Author = "Author One",
                PublicationDate = new DateTime(2020, 1, 1)
            },
            new Models.Book
            {
                Title = "Book Two",
                Author = "Author Two",
                PublicationDate = new DateTime(2021, 2, 2)
            });

        await context.SaveChangesAsync();

        var service = CreateBookService(context);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Contains(
            result,
            book => book.Title == "Book One");

        Assert.Contains(
            result,
            book => book.Title == "Book Two");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoBooksExist()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateBookService(context);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBook_WhenBookExists()
    {
        // Arrange
        await using var context = CreateDbContext();

        var book = new Models.Book
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            PublicationDate = new DateTime(2008, 8, 1)
        };

        context.Books.Add(book);
        await context.SaveChangesAsync();

        var service = CreateBookService(context);

        // Act
        var result = await service.GetByIdAsync(book.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(book.Id, result.Id);
        Assert.Equal("Clean Code", result.Title);
        Assert.Equal("Robert C. Martin", result.Author);
        Assert.Equal(
            new DateTime(2008, 8, 1),
            result.PublicationDate);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenBookDoesNotExist()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateBookService(context);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAndReturnBook()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateBookService(context);

        var request = new CreateBookRequest
        {
            Title = "The Hobbit",
            Author = "J.R.R. Tolkien",
            PublicationDate = new DateTime(1937, 9, 21)
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        Assert.Equal("The Hobbit", result.Title);
        Assert.Equal("J.R.R. Tolkien", result.Author);
        Assert.Equal(
            new DateTime(1937, 9, 21),
            result.PublicationDate);

        var bookInDatabase = await context.Books
            .SingleOrDefaultAsync(b => b.Id == result.Id);

        Assert.NotNull(bookInDatabase);
        Assert.Equal("The Hobbit", bookInDatabase.Title);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBook_WhenBookExists()
    {
        // Arrange
        await using var context = CreateDbContext();

        var book = new Models.Book
        {
            Title = "Old Title",
            Author = "Old Author",
            PublicationDate = new DateTime(2000, 1, 1)
        };

        context.Books.Add(book);
        await context.SaveChangesAsync();

        var service = CreateBookService(context);

        var request = new UpdateBookRequest
        {
            Title = "New Title",
            Author = "New Author",
            PublicationDate = new DateTime(2025, 5, 5)
        };

        // Act
        var result = await service.UpdateAsync(
            book.Id,
            request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(book.Id, result.Id);
        Assert.Equal("New Title", result.Title);
        Assert.Equal("New Author", result.Author);
        Assert.Equal(
            new DateTime(2025, 5, 5),
            result.PublicationDate);

        var updatedBook = await context.Books
            .SingleAsync(b => b.Id == book.Id);

        Assert.Equal("New Title", updatedBook.Title);
        Assert.Equal("New Author", updatedBook.Author);
        Assert.Equal(
            new DateTime(2025, 5, 5),
            updatedBook.PublicationDate);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenBookDoesNotExist()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateBookService(context);

        var request = new UpdateBookRequest
        {
            Title = "New Title",
            Author = "New Author",
            PublicationDate = new DateTime(2025, 5, 5)
        };

        // Act
        var result = await service.UpdateAsync(
            999,
            request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrueAndDeleteBook_WhenBookExists()
    {
        // Arrange
        await using var context = CreateDbContext();

        var book = new Models.Book
        {
            Title = "Book To Delete",
            Author = "Test Author",
            PublicationDate = new DateTime(2020, 1, 1)
        };

        context.Books.Add(book);
        await context.SaveChangesAsync();

        var service = CreateBookService(context);

        // Act
        var result = await service.DeleteAsync(book.Id);

        // Assert
        Assert.True(result);

        var deletedBook = await context.Books
            .SingleOrDefaultAsync(b => b.Id == book.Id);

        Assert.Null(deletedBook);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenBookDoesNotExist()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateBookService(context);

        // Act
        var result = await service.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }
}