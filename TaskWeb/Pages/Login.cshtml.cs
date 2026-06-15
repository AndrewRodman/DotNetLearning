using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskApp.Services;

namespace TaskWeb.Pages
{
    public class LoginModel(ITaskApiService api) : PageModel
    {
        [BindProperty]
        public string Username { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var auth = await api.LoginAsync(Username, Password);
                if (auth is null)
                {
                    ErrorMessage = "Login failed. Check username/password.";
                    return Page();
                }
                return RedirectToPage("/Tasks");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Could not reach API. Is TaskApi running? ({ex.Message})";
                return Page();
            }
        }
    }
}
