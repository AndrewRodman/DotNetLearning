using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using TaskApp.Models;
using TaskApp.Services;

namespace TaskWeb.Pages
{
    public class EditModel(ITaskApiService api, ISessionContext session) : PageModel
    {
        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public string Title { get; set; } = "";

        [BindProperty]
        public string? Description { get; set; } = "";

        [BindProperty]
        public bool IsComplete { get; set; }

        [BindProperty]
        public bool HasDueDate { get; set; }

        [BindProperty]
        public DateTime? DueDate { get; set; }

        public string? ErrorMessage { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!session.IsLoggedIn)
                return RedirectToPage("/Login");

            var task = await api.GetTaskByIdAsync(id);

            if (task != null)
            {
                Id = task.Id;
                Title = task.Title;
                Description = task.Description;
                IsComplete = task.IsComplete;
                HasDueDate = task.DueDate is not null;
                DueDate = task.DueDate;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Please enter a valid title";
                return Page();
            }

            DateTime? dueDate = HasDueDate ? DueDate?.Date : null;

            var updated = await api.UpdateTaskAsync(new TaskItem
            {
                Id = Id,
                Title = Title,
                Description = Description,
                IsComplete = IsComplete,
                DueDate =dueDate
            });

            if (updated is null)
            {
                ErrorMessage = "Could not update task.";
                return Page();
            }

            return RedirectToPage("/Tasks");
        }
    }
}
