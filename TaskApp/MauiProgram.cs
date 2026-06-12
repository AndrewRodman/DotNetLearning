using Microsoft.Extensions.Logging;
using TaskApp.Configuration;
using TaskApp.Pages;
using TaskApp.Services;

namespace TaskApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<SessionContext>();
        builder.Services.AddSingleton<HttpClient>(_ => new HttpClient
        {
            BaseAddress = new Uri(ApiSettings.BaseUrl)
        });
        builder.Services.AddSingleton<ITaskApiService, TaskApiService>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<TasksPage>();
        builder.Services.AddTransient<EditTaskPage>();
        builder.Services.AddSingleton<AppShell>();

        return builder.Build();
    }
}