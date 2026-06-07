using TaskApp.Pages;

namespace TaskApp;

public partial class AppShell : Shell
{
    public AppShell(LoginPage loginPage, TasksPage tasksPage)
    {
        InitializeComponent();

        Items.Add(new ShellContent
        {
            Title = "Login",
            Route = "login",
            Content = loginPage
        });

        Items.Add(new ShellContent
        {
            Title = "Tasks",
            Route = "tasks",
            Content = tasksPage
        });
    }
}