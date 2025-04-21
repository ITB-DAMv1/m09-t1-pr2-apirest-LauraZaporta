using Client.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Pages.Shared.Login
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ILogger _logger;

        [BindProperty]
        public LoginDTO Login { get; set; } = new();
        public string? ErrorMessage { get; set; }
        [TempData]
        public string? SuccessMessage { get; set; }


        public LoginModel(IHttpClientFactory httpClient, ILogger<LoginModel> logging)
        {
            _httpClient = httpClient;
            _logger = logging;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                var client = _httpClient.CreateClient("ApiGames");
                var response = await client.PostAsJsonAsync("api/Auth/login", Login);

                if (response.IsSuccessStatusCode)
                {
                    //La resposta de /api/auth/login es directament el Token fem servir ReadAsStringAsync
                    var token = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(token))
                    {
                        //Guardem en sessió (cookies) el Token amb la clau "AuthToken"
                        HttpContext.Session.SetString("AuthToken", token);
                        _logger.LogInformation("Login susccesfull");
                        return RedirectToPage("/Index");
                    }
                }
                else
                {
                    _logger.LogInformation("Login failed");
                    ErrorMessage = "Credencials incorrectes o accés no autoritzat.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durant el login");
                ErrorMessage = "Error inesperat. Torna-ho a intentar.";
            }

            return Page();
        }
    }
}