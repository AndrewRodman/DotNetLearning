using TaskApp.Models;
using TaskApp.Services;

namespace TaskApp.Pages;

public partial class TasksPage : ContentPage
{
    private readonly ITaskApiService _api;
    private readonly SessionContext _session;
    private List<TaskItem> _tasks = [];
    private bool? _filter =null;
    private bool _isReloading;

    public TasksPage(ITaskApiService api, SessionContext session)
    {
        InitializeComponent();
        _api = api;
        _session = session;
        FilterPicker.SelectedIndex = 0; 
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
        if (_isReloading) return;

        if (sender is not CheckBox checkBox || checkBox.BindingContext is not TaskItem task)
        {
            return;
        }

        task.IsComplete = e.Value;
        var updated = await _api.UpdateTaskAsync(task);
        if (updated is null)
        {
            StatusLabel.Text = "Could not update task.";
            StatusLabel.IsVisible = true;
            await LoadTasksAsync();
            return;
        }

        if (_filter is not null && task.IsComplete != _filter)
            await LoadTasksAsync();
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        _session.Clear();
        await Shell.Current.GoToAsync("//login");
    }

    private async Task LoadTasksAsync()
    {
        _isReloading = true;
        try
        {
            _tasks = (await _api.GetTasksAsync(_filter)).ToList();
            TasksCollection.ItemsSource = null;
            TasksCollection.ItemsSource = _tasks;
            StatusLabel.IsVisible = false;
        }
        catch (Exception)
        {
            StatusLabel.Text = "Could not load tasks. Is TaskApi running?";
            StatusLabel.IsVisible = true;
        }
        finally
        {
            _isReloading = false;
        }
    }

    private async void OnEditTaskClicked(object? sender, EventArgs e)
    {
        if (sender is not Button btn || btn.BindingContext is not TaskItem task)
        {
            return;
        }

        await Shell.Current.GoToAsync("editTask", new Dictionary<string, object>
        {
            ["TaskToEdit"] = task
        });
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

    private async void FilterPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if(sender is not Picker picker)
        {  
            return; 
        }

        if(picker.SelectedIndex ==0)
        {
            _filter = null;
        }
        else if(picker.SelectedIndex == 1)
        {
            _filter = false;
        }
        else if (picker.SelectedIndex == 2)
        {
            _filter = true;
        }

        await LoadTasksAsync();
    }
}