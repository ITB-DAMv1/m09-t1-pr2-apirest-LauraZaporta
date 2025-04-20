using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using API.Model;
using API.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        [HttpPost("register/admin")]
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

        // Login
        // -----
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            //Certifiquem que el mail existeix
            var usuari = await _userManager.FindByEmailAsync(model.Email);
            if (usuari == null || !await _userManager.CheckPasswordAsync(usuari, model.Password))
                return Unauthorized("Mail o contrasenya erronis");

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuari.UserName),
                new Claim(ClaimTypes.NameIdentifier, usuari.Id.ToString())
            };

            //Adquirim els rols de l'usuari. Pot tenir mes d'un. En aquest cas 1.
            var roles = await _userManager.GetRolesAsync(usuari);

            if (roles != null && roles.Count > 0)
            {
                foreach (var rol in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, rol));
                }
            }

            return Ok(CreateToken(claims.ToArray()));
        }

        private string CreateToken(Claim[] claims)
        {
            // Carreguem les dades des del appsettings.json
            var jwtConfig = _configuration.GetSection("JwtSettings");
            var secretKey = jwtConfig["Key"];
            var issuer = jwtConfig["Issuer"];
            var audience = jwtConfig["Audience"];
            var expirationMinutes = int.Parse(jwtConfig["ExpirationMinutes"]);

            // Creem la clau i les credencials de signatura
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Construcció del token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds
            );

            // Retornem el token serialitzat
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}