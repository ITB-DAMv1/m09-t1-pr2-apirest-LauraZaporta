using API.Data;
using API.DTOs;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public GamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Inserció de jocs
        // ----------------
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Game>> InsertGame([FromForm] GameDTO gameInput)
        {
            if (gameInput == null)
                return BadRequest("No hi ha dades a inserir; el joc és buit!");

            try
            {
                string? imagePath = null;

                if (gameInput.Image != null && gameInput.Image.Length > 0)
                {
                    // wwwroot per defecte és una carpeta pública
                    var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    // Per si no existeix
                    Directory.CreateDirectory(imagesFolder);

                    // Genera un nom d'archiu amb identificador únic i amb l'extensió de la imatge enviada
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(gameInput.Image.FileName);
                    // Construeix la ruta final del fitxer
                    var filePath = Path.Combine(imagesFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await gameInput.Image.CopyToAsync(stream); // Copia la imatge donada dintre de la ruta filePath
                    }

                    // Guarda la ruta relativa com a URL a la BBDD
                    imagePath = Path.Combine("images", uniqueFileName).Replace("\\", "/");
                }

                var game = new Game
                {
                    Title = gameInput.Title,
                    Description = gameInput.Description,
                    TeamName = gameInput.TeamName,
                    ImagePath = imagePath
                };

                _context.Games.Add(game);
                await _context.SaveChangesAsync();

                return Ok($"Joc {game.Title} inserit correctament!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en la inserció: {ex.Message}");
            }
        }
    }
}