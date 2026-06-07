using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TaskApi.Configuration;
using TaskApi.Data;
using TaskApi.Models;
using TaskApi.Services;

namespace TaskApi.Tests;

public class AuthServiceTests
{
    private static AuthService CreateService(AppDbContext db)
    {
        var settings = Options.Create(new JwtSettings
        {
            Key = "TestSecretKey_AtLeast32CharactersLong!",
            Issuer = "TaskApiTests",
            Audience = "TaskApiTests",
            ExpiresInMinutes = 30
        });

        return new AuthService(db, settings);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task RegisterAsync_ReturnsToken_ForNewUser()
    {
        await using var db = CreateContext();
        var service = CreateService(db);

        var response = await service.RegisterAsync(new RegisterRequest
        {
            Username = "NewUser",
            Password = "Password123"
        });

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response.Token));
        Assert.Equal("newuser", response.Username);
    }

    [Fact]
    public async Task RegisterAsync_ReturnsNull_WhenUsernameExists()
    {
        await using var db = CreateContext();
        var service = CreateService(db);

        await service.RegisterAsync(new RegisterRequest
        {
            Username = "demo",
            Password = "Password123"
        });

        var duplicate = await service.RegisterAsync(new RegisterRequest
        {
            Username = "DEMO",
            Password = "OtherPassword456"
        });

        Assert.Null(duplicate);
    }

    [Fact]
    public async Task LoginAsync_ReturnsToken_WithValidCredentials()
    {
        await using var db = CreateContext();
        var service = CreateService(db);

        await service.RegisterAsync(new RegisterRequest
        {
            Username = "demo",
            Password = "Password123"
        });

        var response = await service.LoginAsync(new LoginRequest
        {
            Username = "demo",
            Password = "Password123"
        });

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response.Token));
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WithWrongPassword()
    {
        await using var db = CreateContext();
        var service = CreateService(db);

        await service.RegisterAsync(new RegisterRequest
        {
            Username = "demo",
            Password = "Password123"
        });

        var response = await service.LoginAsync(new LoginRequest
        {
            Username = "demo",
            Password = "WrongPassword"
        });

        Assert.Null(response);
    }
}