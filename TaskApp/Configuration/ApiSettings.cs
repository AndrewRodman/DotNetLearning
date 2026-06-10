namespace TaskApp.Configuration;

public static class ApiSettings
{
#if DEBUG
    public const string BaseUrl = "http://localhost:5046/";
#else
    public const string BaseUrl = "https://taskapi-andrew.azurewebsites.net/";
#endif
}