using Client.DTOs;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Pages
{
    public class DetailPageModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty(SupportsGet = true)] // Rep la ID per URL
        public int Id { get; set; }
        public GameGetDTO? Game { get; set; }

        public DetailPageModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGet()
        {
            var client = _httpClientFactory.CreateClient("ApiGames");

            try
            {
                //var response = await client.GetAsync("api/Films/hi");
                var response = await client.GetAsync($"api/Games/{Id}");
                //var response = await client.GetFromJsonAsAsyncEnumerable<List<Film>>("Films",);
                if (response == null || !response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error de carrega de dades!");
                }
                else
                {
                    //_logger.LogError(await response.Content.ReadAsStringAsync());
                    var json = await response.Content.ReadAsStringAsync();
                    Game = JsonSerializer.Deserialize<GameGetDTO>
                        (json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
            }
        }
    }
}
