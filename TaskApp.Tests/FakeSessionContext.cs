using TaskApp.Services;

namespace TaskApp.Tests;

public sealed class FakeSessionContext : ISessionContext
{
    public string? Token { get; private set; }
    public string? Username { get; private set; }
    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(Token);

    public void Save(string token, string username)
    {
        Token = token;
        Username = username;
    }

    public void Clear()
    {
        Token = null;
        Username = null;
    }
}