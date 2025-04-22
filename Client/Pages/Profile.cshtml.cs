using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Pages
{
    public class ProfileModel : PageModel
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public bool LoggedIn { get; set; } = false;

        public ProfileModel()
        {
        }

        public IActionResult OnGet()
        {
            Email = HttpContext.Session.GetString("Email");
            UserName = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(Email))
            {
                LoggedIn = true;
            }
            else
            {
                LoggedIn = false;
            }
            return Page();
        }
    }
}