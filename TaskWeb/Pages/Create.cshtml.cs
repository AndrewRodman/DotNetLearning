using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskApp.Services;

namespace TaskWeb.Pages
{
    public class CreateModel(ITaskApiService api, ISessionContext session) : PageModel
    {
        [BindProperty]
        public string Title { get; set; } = "";

        [BindProperty]
        public string Description { get; set; } = "";

        [BindProperty]
        public bool HasDueDate { get; set; }

        [BindProperty]
        public DateTime? DueDate { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!session.IsLoggedIn)
                return RedirectToPage("/Login");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() { 
            if(string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Please enter a valid title";
                return Page();
            }

            DateTime? dueDate = HasDueDate ? DueDate?.Date : null;

            var created =await api.CreateTaskAsync(Title.Trim(), Description?.Trim(), dueDate);

            if (created is null)
            {
                ErrorMessage = "Could not create task.";
                return Page();
            }

            return RedirectToPage("/Tasks");
        }
    }
}
