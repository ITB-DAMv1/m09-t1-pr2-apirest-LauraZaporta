using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using API.Model;
using API.DTOs;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        public AuthController(UserManager<User> userManager, ILogger<AuthController> logger, IConfiguration configuration)
        {
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
        }

        // Register
        // --------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            var usuari = new User { UserName = model.Username, Email = model.Email };
            var resultat = await _userManager.CreateAsync(usuari, model.Password);

            if (resultat.Succeeded)
                return Ok("Usuari registrat");

            return BadRequest(resultat.Errors);
        }
        [HttpPost("admin/register")]
        public async Task<IActionResult> AdminRegister([FromBody] RegisterDTO model)
        {
            var usuari = new User { UserName = model.Username, Email = model.Email };
            var resultat = await _userManager.CreateAsync(usuari, model.Password);
            var resultatRol = new IdentityResult();

            if (resultat.Succeeded)
            {
                resultatRol = await _userManager.AddToRoleAsync(usuari, "Admin");
            }

            if (resultat.Succeeded && resultatRol.Succeeded)
                return Ok("Usuari registrat");

            return BadRequest(resultat.Errors);
        }
    }
}