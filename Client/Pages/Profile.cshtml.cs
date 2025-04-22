using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Pages
{
    public class ProfileModel : PageModel
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool LoggedIn { get; set; } = false;

        public ProfileModel()
        {
        }

        public IActionResult OnGet()
        {
            UserName = HttpContext.Session.GetString("UserName");
            Email = HttpContext.Session.GetString("Email");

            if (!string.IsNullOrEmpty(UserName))
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