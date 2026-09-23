using BookQuoteApi.Data;
using BookQuoteApi.DTOs.Auth;
using BookQuoteApi.Models;
using BookQuoteApi.Security;
using BookQuoteApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookQuoteApi.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext context,
        JwtService jwtService,
        ILogger<AuthService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var username = request.Username.Trim();

        _logger.LogInformation(
            "Register attempt for username {Username}",
            username);

        var usernameExists = await _context.Users
            .AnyAsync(u => u.Username == username);

        if (usernameExists)
        {
            _logger.LogWarning(
                "Registration failed because username {Username} already exists",
                username);

            throw new InvalidOperationException(
                "Username already exists.");
        }

        var user = new User
        {
            Username = username
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "User registered successfully with UserId {UserId}",
            user.Id);

        return new AuthResponse
        {
            Token = _jwtService.GenerateToken(user),
            Username = user.Username
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var username = request.Username.Trim();

        _logger.LogInformation(
            "Login attempt for username {Username}",
            username);

        var user = await _context.Users
            .SingleOrDefaultAsync(u => u.Username == username);

        if (user is null)
        {
            _logger.LogWarning(
                "Login failed for username {Username}",
                username);

            throw new UnauthorizedAccessException(
                "Invalid username or password.");
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning(
                "Login failed for username {Username}",
                username);

            throw new UnauthorizedAccessException(
                "Invalid username or password.");
        }

        _logger.LogInformation(
            "Login successful for UserId {UserId}",
            user.Id);

        return new AuthResponse
        {
            Token = _jwtService.GenerateToken(user),
            Username = user.Username
        };
    }
}
