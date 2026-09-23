using System.Net;
using System.Net.Http.Json;
using BookQuoteApi.DTOs.Auth;

namespace BookQuoteApi.Tests;

public class AuthControllerIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidRequest_ReturnsOkAndToken()
    {
        var request = new RegisterRequest
        {
            Username = $"integrationuser_{Guid.NewGuid():N}",
            Password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            request);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var result = await response.Content
            .ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(result);
        Assert.Equal(request.Username, result.Username);
        Assert.False(
            string.IsNullOrWhiteSpace(result.Token));
    }

    [Fact]
    public async Task Register_WithExistingUsername_ReturnsConflict()
    {
        var username = $"integrationuser_{Guid.NewGuid():N}";

        var request = new RegisterRequest
        {
            Username = username,
            Password = "Password123!"
        };

        var firstResponse = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            request);

        Assert.Equal(
            HttpStatusCode.OK,
            firstResponse.StatusCode);

        var secondResponse = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            request);

        Assert.Equal(
            HttpStatusCode.Conflict,
            secondResponse.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkAndToken()
    {
        var username =
            $"integrationuser_{Guid.NewGuid():N}";

        var password = "Password123!";

        var registerRequest = new RegisterRequest
        {
            Username = username,
            Password = password
        };

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            registerRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            registerResponse.StatusCode);

        var loginRequest = new LoginRequest
        {
            Username = username,
            Password = password
        };

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/Auth/login",
            loginRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            loginResponse.StatusCode);

        var result = await loginResponse.Content
            .ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(result);
        Assert.Equal(username, result.Username);
        Assert.False(
            string.IsNullOrWhiteSpace(result.Token));
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var username =
            $"integrationuser_{Guid.NewGuid():N}";

        var registerRequest = new RegisterRequest
        {
            Username = username,
            Password = "Password123!"
        };

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            registerRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            registerResponse.StatusCode);

        var loginRequest = new LoginRequest
        {
            Username = username,
            Password = "WrongPassword123!"
        };

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/Auth/login",
            loginRequest);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            loginResponse.StatusCode);
    }

    [Fact]
    public async Task Register_WithInvalidRequest_ReturnsBadRequest()
    {
        var request = new RegisterRequest
        {
            Username = "",
            Password = ""
        };

        var response = await _client.PostAsJsonAsync(
            "/api/Auth/register",
            request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Login_WithInvalidRequest_ReturnsBadRequest()
    {
        var request = new LoginRequest
        {
            Username = "",
            Password = ""
        };

        var response = await _client.PostAsJsonAsync(
            "/api/Auth/login",
            request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
}