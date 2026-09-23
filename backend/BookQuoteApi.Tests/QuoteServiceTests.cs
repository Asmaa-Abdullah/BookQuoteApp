using BookQuoteApi.Data;
using BookQuoteApi.DTOs.Quotes;
using BookQuoteApi.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookQuoteApi.Tests;

public class QuoteServiceTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static QuoteService CreateQuoteService(
        AppDbContext context)
    {
        return new QuoteService(context);
    }

    [Fact]
    public async Task GetMyQuotesAsync_ShouldReturnOnlyCurrentUsersQuotes()
    {
        // Arrange
        await using var context = CreateDbContext();

        context.Quotes.AddRange(
            new Models.Quote
            {
                Text = "User 1 quote",
                UserId = 1
            },
            new Models.Quote
            {
                Text = "User 2 quote",
                UserId = 2
            },
            new Models.Quote
            {
                Text = "Another User 1 quote",
                UserId = 1
            });

        await context.SaveChangesAsync();

        var service = CreateQuoteService(context);

        // Act
        var result = await service.GetMyQuotesAsync(1);

        // Assert
        Assert.Equal(2, result.Count);

        Assert.All(
            result,
            quote => Assert.Contains(
                quote.Text,
                new[]
                {
                    "User 1 quote",
                    "Another User 1 quote"
                }));

        Assert.DoesNotContain(
            result,
            quote => quote.Text == "User 2 quote");
    }

    [Fact]
    public async Task GetMyQuotesAsync_ShouldReturnEmptyList_WhenUserHasNoQuotes()
    {
        // Arrange
        await using var context = CreateDbContext();

        context.Quotes.Add(
            new Models.Quote
            {
                Text = "Other user's quote",
                UserId = 2
            });

        await context.SaveChangesAsync();

        var service = CreateQuoteService(context);

        // Act
        var result = await service.GetMyQuotesAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateQuoteForCurrentUser()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateQuoteService(context);

        var request = new CreateQuoteRequest
        {
            Text = "This is my favorite quote."
        };

        // Act
        var result = await service.CreateAsync(
            1,
            request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        Assert.Equal(
            "This is my favorite quote.",
            result.Text);

        var quoteInDatabase = await context.Quotes
            .SingleOrDefaultAsync(q => q.Id == result.Id);

        Assert.NotNull(quoteInDatabase);
        Assert.Equal(1, quoteInDatabase.UserId);
        Assert.Equal(
            "This is my favorite quote.",
            quoteInDatabase.Text);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateQuote_WhenQuoteBelongsToUser()
    {
        // Arrange
        await using var context = CreateDbContext();

        var quote = new Models.Quote
        {
            Text = "Old quote",
            UserId = 1
        };

        context.Quotes.Add(quote);
        await context.SaveChangesAsync();

        var service = CreateQuoteService(context);

        var request = new UpdateQuoteRequest
        {
            Text = "Updated quote"
        };

        // Act
        var result = await service.UpdateAsync(
            1,
            quote.Id,
            request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(quote.Id, result.Id);
        Assert.Equal("Updated quote", result.Text);

        var updatedQuote = await context.Quotes
            .SingleAsync(q => q.Id == quote.Id);

        Assert.Equal("Updated quote", updatedQuote.Text);
        Assert.Equal(1, updatedQuote.UserId);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenQuoteBelongsToAnotherUser()
    {
        // Arrange
        await using var context = CreateDbContext();

        var quote = new Models.Quote
        {
            Text = "Private quote",
            UserId = 2
        };

        context.Quotes.Add(quote);
        await context.SaveChangesAsync();

        var service = CreateQuoteService(context);

        var request = new UpdateQuoteRequest
        {
            Text = "Hacked quote"
        };

        // Act
        var result = await service.UpdateAsync(
            1,
            quote.Id,
            request);

        // Assert
        Assert.Null(result);

        var unchangedQuote = await context.Quotes
            .SingleAsync(q => q.Id == quote.Id);

        Assert.Equal("Private quote", unchangedQuote.Text);
        Assert.Equal(2, unchangedQuote.UserId);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenQuoteDoesNotExist()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateQuoteService(context);

        var request = new UpdateQuoteRequest
        {
            Text = "Updated quote"
        };

        // Act
        var result = await service.UpdateAsync(
            1,
            999,
            request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteQuote_WhenQuoteBelongsToUser()
    {
        // Arrange
        await using var context = CreateDbContext();

        var quote = new Models.Quote
        {
            Text = "Quote to delete",
            UserId = 1
        };

        context.Quotes.Add(quote);
        await context.SaveChangesAsync();

        var service = CreateQuoteService(context);

        // Act
        var result = await service.DeleteAsync(
            1,
            quote.Id);

        // Assert
        Assert.True(result);

        var deletedQuote = await context.Quotes
            .SingleOrDefaultAsync(q => q.Id == quote.Id);

        Assert.Null(deletedQuote);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenQuoteBelongsToAnotherUser()
    {
        // Arrange
        await using var context = CreateDbContext();

        var quote = new Models.Quote
        {
            Text = "Private quote",
            UserId = 2
        };

        context.Quotes.Add(quote);
        await context.SaveChangesAsync();

        var service = CreateQuoteService(context);

        // Act
        var result = await service.DeleteAsync(
            1,
            quote.Id);

        // Assert
        Assert.False(result);

        var quoteStillExists = await context.Quotes
            .SingleOrDefaultAsync(q => q.Id == quote.Id);

        Assert.NotNull(quoteStillExists);
        Assert.Equal("Private quote", quoteStillExists.Text);
        Assert.Equal(2, quoteStillExists.UserId);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenQuoteDoesNotExist()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateQuoteService(context);

        // Act
        var result = await service.DeleteAsync(
            1,
            999);

        // Assert
        Assert.False(result);
    }
}