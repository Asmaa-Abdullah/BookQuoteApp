using BookQuoteApi.Data;
using BookQuoteApi.DTOs.Auth;
using BookQuoteApi.Models;
using BookQuoteApi.Security;
using BookQuoteApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit;

namespace BookQuoteApi.Tests;

public class AuthServiceTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static JwtService CreateJwtService()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "ThisIsASecretKeyForTesting123456789",
                ["Jwt:Issuer"] = "BookQuoteApi",
                ["Jwt:Audience"] = "BookQuoteClient",
                ["Jwt:ExpiresInMinutes"] = "60"
            })
            .Build();

        return new JwtService(configuration);
    }

    private static AuthService CreateAuthService(
        AppDbContext context)
    {
        var jwtService = CreateJwtService();

        var logger = LoggerFactory
            .Create(builder => { })
            .CreateLogger<AuthService>();

        return new AuthService(
            context,
            jwtService,
            logger);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUserAndReturnToken()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateAuthService(context);

        var request = new RegisterRequest
        {
            Username = "testuser",
            Password = "Password123!"
        };

        // Act
        var result = await service.RegisterAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));

        var user = await context.Users
            .SingleOrDefaultAsync(u => u.Username == "testuser");

        Assert.NotNull(user);
        Assert.NotEqual(0, user.Id);
        Assert.NotEqual(
            request.Password,
            user.PasswordHash);
    }

    [Fact]
    public async Task RegisterAsync_ShouldTrimUsername()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateAuthService(context);

        var request = new RegisterRequest
        {
            Username = "  testuser  ",
            Password = "Password123!"
        };

        // Act
        var result = await service.RegisterAsync(request);

        // Assert
        Assert.Equal("testuser", result.Username);

        var user = await context.Users
            .SingleOrDefaultAsync(u => u.Username == "testuser");

        Assert.NotNull(user);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrow_WhenUsernameAlreadyExists()
    {
        // Arrange
        await using var context = CreateDbContext();

        var existingUser = new User
        {
            Username = "testuser",
            PasswordHash = "existing-hash"
        };

        context.Users.Add(existingUser);
        await context.SaveChangesAsync();

        var service = CreateAuthService(context);

        var request = new RegisterRequest
        {
            Username = "testuser",
            Password = "Password123!"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RegisterAsync(request));

        Assert.Equal(
            "Username already exists.",
            exception.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        await using var context = CreateDbContext();

        var user = new User
        {
            Username = "testuser"
        };

        var passwordHasher =
            new Microsoft.AspNetCore.Identity.PasswordHasher<User>();

        user.PasswordHash = passwordHasher.HashPassword(
            user,
            "Password123!");

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = CreateAuthService(context);

        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "Password123!"
        };

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenUsernameDoesNotExist()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateAuthService(context);

        var request = new LoginRequest
        {
            Username = "unknownuser",
            Password = "Password123!"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.LoginAsync(request));

        Assert.Equal(
            "Invalid username or password.",
            exception.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenPasswordIsInvalid()
    {
        // Arrange
        await using var context = CreateDbContext();

        var user = new User
        {
            Username = "testuser"
        };

        var passwordHasher =
            new Microsoft.AspNetCore.Identity.PasswordHasher<User>();

        user.PasswordHash = passwordHasher.HashPassword(
            user,
            "CorrectPassword123!");

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = CreateAuthService(context);

        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "WrongPassword123!"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.LoginAsync(request));

        Assert.Equal(
            "Invalid username or password.",
            exception.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldTrimUsername()
    {
        // Arrange
        await using var context = CreateDbContext();

        var user = new User
        {
            Username = "testuser"
        };

        var passwordHasher =
            new Microsoft.AspNetCore.Identity.PasswordHasher<User>();

        user.PasswordHash = passwordHasher.HashPassword(
            user,
            "Password123!");

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = CreateAuthService(context);

        var request = new LoginRequest
        {
            Username = "  testuser  ",
            Password = "Password123!"
        };

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        Assert.Equal("testuser", result.Username);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
    }
}