using TaskApp.Models;
using TaskApp.Services;

namespace TaskApp.Pages;

public partial class TasksPage : ContentPage
{
    private readonly ITaskApiService _api;
    private readonly SessionContext _session;
    private List<TaskItem> _tasks = [];

    public TasksPage(ITaskApiService api, SessionContext session)
    {
        InitializeComponent();
        _api = api;
        _session = session;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadTasksAsync();
    }

    private async void OnRefreshClicked(object? sender, EventArgs e)
    {
        await LoadTasksAsync();
    }

    private async void OnAddTaskClicked(object? sender, EventArgs e)
    {
        StatusLabel.IsVisible = false;

        if (string.IsNullOrWhiteSpace(NewTitleEntry.Text))
        {
            StatusLabel.Text = "Enter a title.";
            StatusLabel.IsVisible = true;
            return;
        }

        var created = await _api.CreateTaskAsync(NewTitleEntry.Text, NewDescriptionEntry.Text);
        if (created is null)
        {
            StatusLabel.Text = "Could not create task.";
            StatusLabel.IsVisible = true;
            return;
        }

        NewTitleEntry.Text = string.Empty;
        NewDescriptionEntry.Text = string.Empty;
        await LoadTasksAsync();
    }

    private async void OnTaskCompleteChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (sender is not CheckBox checkBox || checkBox.BindingContext is not TaskItem task)
        {
            return;
        }

        task.IsComplete = e.Value;
        await _api.UpdateTaskAsync(task);
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        _session.Clear();
        await Shell.Current.GoToAsync("//login");
    }

    private async Task LoadTasksAsync()
    {
        try
        {
            _tasks = (await _api.GetTasksAsync()).ToList();
            TasksCollection.ItemsSource = null;
            TasksCollection.ItemsSource = _tasks;
            StatusLabel.IsVisible = false;
        }
        catch (Exception)
        {
            StatusLabel.Text = "Could not load tasks. Is TaskApi running?";
            StatusLabel.IsVisible = true;
        }
    }

    private async void OnDeleteTaskClicked(object? sender, EventArgs e)
    {
        if (sender is not Button btn || btn.BindingContext is not TaskItem task)
        {
            return;
        }

        var confirmed = await DisplayAlertAsync("Delete task?", task.Title, "Delete", "Cancel");

        if (confirmed) 
        {
            var deleted = await _api.DeleteTaskAsync(task.Id);
            if (!deleted)
            {
                StatusLabel.Text = "Could not delete task.";
                StatusLabel.IsVisible = true;
                return;
            }

            await LoadTasksAsync();
        }
    }
}