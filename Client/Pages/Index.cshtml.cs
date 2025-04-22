using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Client.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public bool LoggedIn { get; private set; } = false;
        public List<GameGetDTO> Games { get; set; } = new List<GameGetDTO>();
        public List<GameGetDTO> TopGames { get; set; } = new List<GameGetDTO>();

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGet()
        {
            var client = _httpClientFactory.CreateClient("ApiGames");
            var token = HttpContext.Session.GetString("AuthToken");

            if (!string.IsNullOrEmpty(token))
            {
                LoggedIn = true;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                LoggedIn = false;
            }

            try
            {
                //var response = await client.GetAsync("api/Films/hi");
                var response = await client.GetAsync("api/Games/allGames");
                //var response = await client.GetFromJsonAsAsyncEnumerable<List<Film>>("Films",);
                if (response == null || !response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error de carrega de dades!");
                }
                else
                {
                    //_logger.LogError(await response.Content.ReadAsStringAsync());
                    var json = await response.Content.ReadAsStringAsync();
                    Games = JsonSerializer.Deserialize<List<GameGetDTO>>
                        (json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                }
                Games = Games.OrderByDescending(g => g.VoteCount).ToList();
                TopGames = Games.Take(10).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
            }
        }
        public async Task<IActionResult> OnPostVoteAsync(int id)
        {
            var token = HttpContext.Session.GetString("AuthToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login/Login");
            }

            var client = _httpClientFactory.CreateClient("ApiGames");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var apiUrl = $"api/Games/vote/{id}";
                var request = new HttpRequestMessage(HttpMethod.Put, apiUrl);
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Vot afegit correctament per al joc ID: {id}");
                    TempData["SuccessMessage"] = "Has votat!";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"Error en afegir vot per al joc ID {id}. Estat: {response.StatusCode}. Detalls API: {errorContent}");

                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        TempData["ErrorMessage"] = $"Error: No s'ha pogut afegir el vot ({errorContent})";
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        TempData["ErrorMessage"] = "Error d'autorització. Si us plau, torna a iniciar sessió.";
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        TempData["ErrorMessage"] = "Error: El joc no s'ha trobat.";
                    }
                    else if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        TempData["ErrorMessage"] = "Ja has votat aquest joc.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Error en registrar el vot (Codi: {response.StatusCode}).";
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Error de xarxa en afegir vot per a Joc ID: {GameId}", id);
                TempData["ErrorMessage"] = "Error de connexió en intentar votar.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperat a OnPostAddVoteAsync per a Joc ID: {GameId}", id);
                TempData["ErrorMessage"] = "Error inesperat en processar el teu vot.";
            }

            return RedirectToPage();
        }
    }
}