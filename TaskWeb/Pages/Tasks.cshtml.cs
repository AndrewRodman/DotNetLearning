using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskApp.Models;
using TaskApp.Services;

namespace TaskWeb.Pages
{
    public class TasksModel(ITaskApiService api, ISessionContext session)  : PageModel
    {
        public IReadOnlyList<TaskItem> Tasks { get; set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {
            if (!session.IsLoggedIn)
                return RedirectToPage("/Login");

            Tasks = await api.GetTasksAsync();
            return Page();
        }
    }
}
