using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookQuoteApi.DTOs.Books;

namespace BookQuoteApi.Tests;

public class BooksControllerIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BooksControllerIntegrationTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_WithoutAuthentication_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/books");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WithAuthentication_ReturnsOk()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("/api/books");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithAuthentication_ReturnsCreated()
    {
        await AuthenticateAsync();

        var request = new CreateBookRequest
        {
            Title = "Integration Test Book",
            Author = "Test Author",
            PublicationDate = new DateTime(2020, 1, 1)
        };

        var response = await _client.PostAsJsonAsync(
            "/api/books",
            request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var book = await response.Content
            .ReadFromJsonAsync<BookResponse>();

        Assert.NotNull(book);
        Assert.Equal(request.Title, book.Title);
        Assert.Equal(request.Author, book.Author);
        Assert.True(book.Id > 0);
    }

    [Fact]
    public async Task GetById_WithAuthentication_ReturnsOk()
    {
        await AuthenticateAsync();

        var createRequest = new CreateBookRequest
        {
            Title = "Get By Id Test",
            Author = "Test Author",
            PublicationDate = new DateTime(2021, 1, 1)
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/books",
            createRequest);

        var createdBook = await createResponse.Content
            .ReadFromJsonAsync<BookResponse>();

        Assert.NotNull(createdBook);

        var response = await _client.GetAsync(
            $"/api/books/{createdBook.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var book = await response.Content
            .ReadFromJsonAsync<BookResponse>();

        Assert.NotNull(book);
        Assert.Equal(createdBook.Id, book.Id);
        Assert.Equal(createRequest.Title, book.Title);
    }

    [Fact]
    public async Task GetById_WhenBookDoesNotExist_ReturnsNotFound()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("/api/books/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_WithAuthentication_ReturnsOk()
    {
        await AuthenticateAsync();

        var createRequest = new CreateBookRequest
        {
            Title = "Original Title",
            Author = "Original Author",
            PublicationDate = new DateTime(2020, 1, 1)
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/books",
            createRequest);

        var createdBook = await createResponse.Content
            .ReadFromJsonAsync<BookResponse>();

        Assert.NotNull(createdBook);

        var updateRequest = new UpdateBookRequest
        {
            Title = "Updated Title",
            Author = "Updated Author",
            PublicationDate = new DateTime(2025, 1, 1)
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/books/{createdBook.Id}",
            updateRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedBook = await response.Content
            .ReadFromJsonAsync<BookResponse>();

        Assert.NotNull(updatedBook);
        Assert.Equal("Updated Title", updatedBook.Title);
        Assert.Equal("Updated Author", updatedBook.Author);
        Assert.Equal(
            new DateTime(2025, 1, 1),
            updatedBook.PublicationDate);
    }

    [Fact]
    public async Task Update_WhenBookDoesNotExist_ReturnsNotFound()
    {
        await AuthenticateAsync();

        var request = new UpdateBookRequest
        {
            Title = "Updated",
            Author = "Author",
            PublicationDate = new DateTime(2025, 1, 1)
        };

        var response = await _client.PutAsJsonAsync(
            "/api/books/999999",
            request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WithAuthentication_ReturnsNoContent()
    {
        await AuthenticateAsync();

        var createRequest = new CreateBookRequest
        {
            Title = "Delete Test",
            Author = "Test Author",
            PublicationDate = new DateTime(2020, 1, 1)
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/books",
            createRequest);

        var createdBook = await createResponse.Content
            .ReadFromJsonAsync<BookResponse>();

        Assert.NotNull(createdBook);

        var response = await _client.DeleteAsync(
            $"/api/books/{createdBook.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync(
            $"/api/books/{createdBook.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenBookDoesNotExist_ReturnsNotFound()
    {
        await AuthenticateAsync();

        var response = await _client.DeleteAsync(
            "/api/books/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task AuthenticateAsync()
    {
        var username = $"booktest_{Guid.NewGuid():N}";

        var registerRequest = new
        {
            Username = username,
            Password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            registerRequest);

        var body = await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"REGISTER STATUS: {(int)response.StatusCode}");

        Console.WriteLine(
            $"REGISTER BODY: {body}");

        response.EnsureSuccessStatusCode();

        var authResponse = await response.Content
            .ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(authResponse);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                authResponse.Token);
    }

    private sealed class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}