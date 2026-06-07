using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskApi.Configuration;
using TaskApi.Data;
using TaskApi.Models;

namespace TaskApi.Services;

public class AuthService(AppDbContext db, IOptions<JwtSettings> jwtOptions) : IAuthService
{
    private readonly JwtSettings _jwt = jwtOptions.Value;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var username = request.Username.Trim().ToLowerInvariant();

        if (await db.Users.AnyAsync(u => u.Username == username))
        {
            return null;
        }

        var user = new User { Username = username };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return CreateToken(user);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var username = request.Username.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user is null)
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return CreateToken(user);
    }

    private AuthResponse CreateToken(User user)
    {
        var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpiresInMinutes);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Username = user.Username,
            ExpiresAt = expires
        };
    }
}