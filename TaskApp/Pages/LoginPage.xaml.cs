using TaskApp.Services;

namespace TaskApp.Pages;

public partial class LoginPage : ContentPage
{
    private readonly ITaskApiService _api;

    public LoginPage(ITaskApiService api)
    {
        InitializeComponent();
        _api = api;
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        await AuthenticateAsync(() => _api.LoginAsync(UsernameEntry.Text, PasswordEntry.Text));
    }

    private async void OnRegisterClicked(object? sender, EventArgs e)
    {
        await AuthenticateAsync(() => _api.RegisterAsync(UsernameEntry.Text, PasswordEntry.Text));
    }

    private async Task AuthenticateAsync(Func<Task<Models.AuthResponse?>> action)
    {
        ErrorLabel.IsVisible = false;
        var auth = await action();

        if (auth is null)
        {
            ErrorLabel.Text = "Login failed. Is TaskApi running? Check username/password.";
            ErrorLabel.IsVisible = true;
            return;
        }

        await Shell.Current.GoToAsync("//tasks");
    }
}