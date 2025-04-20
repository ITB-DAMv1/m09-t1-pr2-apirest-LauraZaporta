using API.Data;
using API.DTOs;
using API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // Edició de jocs
        // --------------
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult<Game>> UpdateGame(GameUpdateDTO gameInput)
        {
            if (gameInput == null) { return BadRequest("No hi ha dades a actualitzar; el joc és buit!"); }

            try
            {
                var games = await _context.Games.ToListAsync();
                var game = games.Where(g => g.Title == gameInput.GameToUpdate).FirstOrDefault();
                if (game == null) { return NotFound("El joc a actualitzar no trobat"); }

                if (gameInput.Title != null) { game.Title = gameInput.Title; };
                if (gameInput.Description != null) { game.Description = gameInput.Description; };
                if (gameInput.TeamName != null) { game.TeamName = gameInput.TeamName; };
                if (gameInput.Image != null && gameInput.Image.Length > 0)
                {
                    var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    Directory.CreateDirectory(imagesFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(gameInput.Image.FileName);
                    var filePath = Path.Combine(imagesFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await gameInput.Image.CopyToAsync(stream); 
                    }

                    game.ImagePath = Path.Combine("images", uniqueFileName).Replace("\\", "/");
                }

                await _context.SaveChangesAsync();
                return Ok($"Joc {game.Title} actualitzat!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en l'actualització: {ex.Message}");
            }
        }

        // Eliminació de jocs
        // ------------------
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<ActionResult<Game>> DeleteGame(int id)
        {
            try
            {
                var game = await _context.Games.FindAsync(id);
                if (game == null) { return NotFound("El joc a eliminar no trobat"); }
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
                return Ok($"Joc {game.Title} eliminat");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en l'eliminació: {ex.Message}");
            }
        }

        // Consulta de jocs
        // ----------------
        // TOTS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetAll()
        {
            try
            {
                var games = await _context.Games.ToListAsync(); //Obtenim tots els jocs de la base de dades
                if (games.Count == 0) { return NotFound("No hi ha cap joc"); }
                return Ok(games); //Tot bé al servidor -> retorna la llista de jocs
            }
            catch (Exception ex)
            {
                return BadRequest("Error al recuperar els jocs: " + ex.Message);
            }
        }
        // Per títol
        [HttpGet("titol")]
        public async Task<ActionResult<IEnumerable<Game>>> GetByName(string name)
        {
            try
            {
                var games = await _context.Games.ToListAsync(); //Obtenim tots els jocs de la base de dades
                if (games.Count == 0) { return NotFound("No hi ha cap joc"); }
                var gamesWithName = games.Where(g => g.Title.ToLower().Contains(name.ToLower())); //Filtrant per nom
                if (gamesWithName.Count() == 0) { return NotFound("No hi ha cap joc amb aquest nom"); }
                return Ok(gamesWithName); //Tot bé al servidor -> retorna la llista de jocs
            }
            catch (Exception ex)
            {
                return BadRequest("Error al recuperar el joc: " + ex.Message);
            }
        }
        // Per equip desenvolupador
        [HttpGet("teamname")]
        public async Task<ActionResult<IEnumerable<Game>>> GetByTeam(string team)
        {
            try
            {
                var games = await _context.Games.ToListAsync(); //Obtenim tots els jocs de la base de dades
                if (games.Count == 0) { return NotFound("No hi ha cap joc"); }
                var gamesWithTeam = games.Where(g => g.TeamName.ToLower().Contains(team.ToLower())); //Filtrant per equip
                if (gamesWithTeam.Count() == 0) { return NotFound("No hi ha cap joc amb aquest equip"); }
                return Ok(gamesWithTeam); //Tot bé al servidor -> retorna la llista de jocs
            }
            catch (Exception ex)
            {
                return BadRequest("Error al recuperar el joc: " + ex.Message);
            }
        }
        // Per id
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetById(int id)
        {
            try
            {
                var gameWithId = await _context.Games.FindAsync(id); //Obtenim tots els jocs de la base de dades
                if (gameWithId == null) { return NotFound("No hi ha cap joc amb aquest id"); }
                return Ok(gameWithId); //Tot bé al servidor -> retorna joc
            }
            catch (Exception ex)
            {
                return BadRequest("Error al recuperar el joc: " + ex.Message);
            }
        }

        // Votació
        // -------
        [Authorize] //Per tots els rols existents
        [HttpPut("vote")]
        public async Task<ActionResult<Game>> VoteGame(int id)
        {
            try
            {
                var game = await _context.Games.FindAsync(id);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

                if (game == null || user == null) { return NotFound("El joc a votar no trobat o l'usuari no és vàlid"); }

                game.UsersWhoVoted.Add(user);
                user.VotedGames.Add(game);
                await _context.SaveChangesAsync();

                return Ok($"Joc {game.Title} votat!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en la votació: {ex.Message}");
            }
        }
    }
}