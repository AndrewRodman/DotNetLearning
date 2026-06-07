namespace TaskApp.Configuration;

public static class ApiSettings
{
#if ANDROID
    public const string BaseUrl = "http://10.0.2.2:5046/";
#else
    public const string BaseUrl = "http://localhost:5046/";
#endif
}