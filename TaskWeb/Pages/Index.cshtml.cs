using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskApp.Services;

namespace TaskWeb.Pages
{
    public class IndexModel() : PageModel
    {
        public IActionResult OnGet() => RedirectToPage("/Login");
    }
}
