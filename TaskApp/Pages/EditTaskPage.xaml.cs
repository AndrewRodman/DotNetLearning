using TaskApp.Models;
using TaskApp.Services;

namespace TaskApp.Pages;

public partial class EditTaskPage : ContentPage, IQueryAttributable
{
    private readonly ITaskApiService _api;
    private TaskItem _task = null!;

    public EditTaskPage(ITaskApiService api)
    {
        InitializeComponent();
        _api = api;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("TaskToEdit", out var value) && value is TaskItem task)
        {
            _task = new TaskItem
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsComplete = task.IsComplete,
                CreatedAt = task.CreatedAt
            };
            TitleEntry.Text = _task.Title;
            DescriptionEntry.Text = _task.Description ?? string.Empty;
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        if (string.IsNullOrWhiteSpace(TitleEntry.Text))
        {
            ErrorLabel.Text = "Enter a title.";
            ErrorLabel.IsVisible = true;
            return;
        }

        _task.Title = TitleEntry.Text.Trim();
        _task.Description = string.IsNullOrWhiteSpace(DescriptionEntry.Text)
            ? null
            : DescriptionEntry.Text.Trim();

        var updated = await _api.UpdateTaskAsync(_task);
        if (updated is null)
        {
            ErrorLabel.Text = "Could not update task.";
            ErrorLabel.IsVisible = true;
            return;
        }

        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}