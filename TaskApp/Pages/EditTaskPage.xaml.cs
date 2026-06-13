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
                CreatedAt = task.CreatedAt,
                DueDate = task.DueDate
            };
            TitleEntry.Text = _task.Title;
            DescriptionEntry.Text = _task.Description ?? string.Empty;

            // After loading _task, if it has a due date:
            if (_task.DueDate is not null)
            {
                DueDateCB.IsChecked = true;
                DueDatePicker.Date = _task.DueDate.Value;
            }
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

        DateTime? dueDate = null;

        if (DueDateCB.IsChecked)
        {
            dueDate = DueDatePicker.Date;
        }

        _task.DueDate= dueDate;

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

    private void DueDateCB_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (DueDateCB.IsChecked)
        {
            DueDatePicker.IsVisible = true;
        }
        else
        {
            DueDatePicker.IsVisible = false;
        }
    }
}