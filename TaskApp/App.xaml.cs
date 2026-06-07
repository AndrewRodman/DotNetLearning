using TaskApp.Services;

namespace TaskApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var services = activationState?.Context?.Services
            ?? throw new InvalidOperationException("MAUI service provider is not available.");

        var shell = services.GetRequiredService<AppShell>();
        var session = services.GetRequiredService<SessionContext>();

        if (session.IsLoggedIn)
        {
            shell.CurrentItem = shell.Items[1];
        }

        return new Window(shell);
    }
}