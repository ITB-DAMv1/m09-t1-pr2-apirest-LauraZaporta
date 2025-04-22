using Client.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Pages.Shared.Register
{
    public class RegisterAdminModel : PageModel
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ILogger _logger;

        [BindProperty]
        public RegisterDTO Register { get; set; } = new();
        [TempData] // Es passa la propietat de pàgina a pàgina
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public RegisterAdminModel(IHttpClientFactory httpClient, ILogger<RegisterModel> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                var client = _httpClient.CreateClient("ApiGames");
                var response = await client.PostAsJsonAsync("api/Auth/register/admin", Register);

                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Admin registrat correctament!";
                    return RedirectToPage("/Login/Login");
                }
                else
                {
                    ErrorMessage = "El registre ha fallat. Comprova les teves dades i recorda si ja t'has registrat prèviament.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durant el registre");
                ErrorMessage = "Error inesperat. Torna-ho a intentar.";
            }

            return Page();
        }
    }
}