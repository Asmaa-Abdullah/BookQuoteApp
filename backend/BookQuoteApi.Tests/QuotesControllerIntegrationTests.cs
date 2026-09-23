using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookQuoteApi.DTOs.Auth;
using BookQuoteApi.DTOs.Quotes;

namespace BookQuoteApi.Tests;

public class QuotesControllerIntegrationTests
: IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public QuotesControllerIntegrationTests(
        CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMyQuotes_WithoutAuthentication_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/quotes");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task GetMyQuotes_WithAuthentication_ReturnsOk()
    {
        await AuthenticateAsync();

        var response = await _client.GetAsync("/api/quotes");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task Create_WithAuthentication_ReturnsOkAndCreatesQuote()
    {
        await AuthenticateAsync();

        var request = new CreateQuoteRequest
        {
            Text = "This is an integration test quote."
        };

        var response = await _client.PostAsJsonAsync(
            "/api/quotes",
            request);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var quote = await response.Content
            .ReadFromJsonAsync<QuoteResponse>();

        Assert.NotNull(quote);
        Assert.True(quote.Id > 0);
        Assert.Equal(request.Text, quote.Text);
    }

    [Fact]
    public async Task GetMyQuotes_ReturnsOnlyCurrentUsersQuotes()
    {
        await AuthenticateAsync();

        var firstUserQuote = new CreateQuoteRequest
        {
            Text = "First user's quote."
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/quotes",
            firstUserQuote);

        Assert.Equal(
            HttpStatusCode.OK,
            createResponse.StatusCode);

        using var secondClient = _factory.CreateClient();

        var secondUsername =
            $"quotetest_{Guid.NewGuid():N}";

        var registerRequest = new RegisterRequest
        {
            Username = secondUsername,
            Password = "Password123!"
        };

        var registerResponse =
            await secondClient.PostAsJsonAsync(
                "/api/auth/register",
                registerRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            registerResponse.StatusCode);

        var authResponse =
            await registerResponse.Content
                .ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(authResponse);

        secondClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                authResponse.Token);

        var secondUserQuote = new CreateQuoteRequest
        {
            Text = "Second user's quote."
        };

        var secondCreateResponse =
            await secondClient.PostAsJsonAsync(
                "/api/quotes",
                secondUserQuote);

        Assert.Equal(
            HttpStatusCode.OK,
            secondCreateResponse.StatusCode);

        var secondGetResponse =
            await secondClient.GetAsync("/api/quotes");

        Assert.Equal(
            HttpStatusCode.OK,
            secondGetResponse.StatusCode);

        var secondQuotes =
            await secondGetResponse.Content
                .ReadFromJsonAsync<List<QuoteResponse>>();

        Assert.NotNull(secondQuotes);

        Assert.Contains(
            secondQuotes,
            q => q.Text == "Second user's quote.");

        Assert.DoesNotContain(
            secondQuotes,
            q => q.Text == "First user's quote.");
    }

    [Fact]
    public async Task Update_WithAuthentication_ReturnsOk()
    {
        await AuthenticateAsync();

        var createRequest = new CreateQuoteRequest
        {
            Text = "Original quote."
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/quotes",
            createRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            createResponse.StatusCode);

        var createdQuote =
            await createResponse.Content
                .ReadFromJsonAsync<QuoteResponse>();

        Assert.NotNull(createdQuote);

        var updateRequest = new UpdateQuoteRequest
        {
            Text = "Updated quote."
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/quotes/{createdQuote.Id}",
            updateRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var updatedQuote =
            await response.Content
                .ReadFromJsonAsync<QuoteResponse>();

        Assert.NotNull(updatedQuote);

        Assert.Equal(
            createdQuote.Id,
            updatedQuote.Id);

        Assert.Equal(
            "Updated quote.",
            updatedQuote.Text);
    }

    [Fact]
    public async Task Update_WhenQuoteDoesNotExist_ReturnsNotFound()
    {
        await AuthenticateAsync();

        var request = new UpdateQuoteRequest
        {
            Text = "Updated quote."
        };

        var response = await _client.PutAsJsonAsync(
            "/api/quotes/999999",
            request);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task Delete_WithAuthentication_ReturnsNoContent()
    {
        await AuthenticateAsync();

        var createRequest = new CreateQuoteRequest
        {
            Text = "Quote to delete."
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/quotes",
            createRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            createResponse.StatusCode);

        var createdQuote =
            await createResponse.Content
                .ReadFromJsonAsync<QuoteResponse>();

        Assert.NotNull(createdQuote);

        var response = await _client.DeleteAsync(
            $"/api/quotes/{createdQuote.Id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var getResponse = await _client.GetAsync(
            "/api/quotes");

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode);

        var quotes =
            await getResponse.Content
                .ReadFromJsonAsync<List<QuoteResponse>>();

        Assert.NotNull(quotes);

        Assert.DoesNotContain(
            quotes,
            q => q.Id == createdQuote.Id);
    }

    [Fact]
    public async Task Delete_WhenQuoteDoesNotExist_ReturnsNotFound()
    {
        await AuthenticateAsync();

        var response = await _client.DeleteAsync(
            "/api/quotes/999999");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    private async Task AuthenticateAsync()
    {
        var username =
            $"quotetest_{Guid.NewGuid():N}";

        var registerRequest = new RegisterRequest
        {
            Username = username,
            Password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            registerRequest);

        response.EnsureSuccessStatusCode();

        var authResponse =
            await response.Content
                .ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(authResponse);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                authResponse.Token);
    }

}
