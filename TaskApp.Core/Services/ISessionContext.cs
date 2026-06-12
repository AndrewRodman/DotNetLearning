namespace TaskApp.Services;

public interface ISessionContext
{
    string? Token { get; }
    string? Username { get; }
    bool IsLoggedIn { get; }
    void Save(string token, string username);
    void Clear();
}