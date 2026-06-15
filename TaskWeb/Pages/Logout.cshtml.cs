using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskApp.Services;

namespace TaskWeb.Pages;

public class LogoutModel(ISessionContext session) : PageModel
{
    public IActionResult OnGet()
    {
        session.Clear();
        return RedirectToPage("/Login");
    }
}