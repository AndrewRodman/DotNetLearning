namespace TaskApp.Services;

public class SessionContext : ISessionContext
{
    private const string TokenKey = "auth_token";
    private const string UsernameKey = "auth_username";

    public string? Token => Preferences.Default.Get(TokenKey, default(string?));
    public string? Username => Preferences.Default.Get(UsernameKey, default(string?));
    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(Token);

    public void Save(string token, string username)
    {
        Preferences.Default.Set(TokenKey, token);
        Preferences.Default.Set(UsernameKey, username);
    }

    public void Clear()
    {
        Preferences.Default.Remove(TokenKey);
        Preferences.Default.Remove(UsernameKey);
    }
}