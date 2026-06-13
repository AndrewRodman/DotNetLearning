namespace TaskApp.Configuration;

public static class ApiSettings
{
    public static string BaseUrl
    {
        get
        {
#if DEBUG
#if ANDROID
            // Android emulator: 10.0.2.2 is the host PC's localhost
            return "http://10.0.2.2:5046/";
#else
            return "http://localhost:5046/";
#endif
#else
            return "https://taskapi-andrew.azurewebsites.net/";
#endif
        }
    }
}