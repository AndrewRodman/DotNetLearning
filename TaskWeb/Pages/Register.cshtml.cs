using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskApp.Services;

namespace TaskWeb.Pages;

public class RegisterModel(ITaskApiService api, ISessionContext session) : PageModel
{
    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        if (session.IsLoggedIn)
        {
            return RedirectToPage("/Tasks");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (session.IsLoggedIn)
        {
            return RedirectToPage("/Tasks");
        }

        try
        {
            var auth = await api.RegisterAsync(Username, Password);
            if (auth is null)
            {
                ErrorMessage = "Registration failed. Username may already exist.";
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