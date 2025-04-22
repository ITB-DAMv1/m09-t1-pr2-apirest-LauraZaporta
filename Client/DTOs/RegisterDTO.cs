using System.ComponentModel.DataAnnotations;

namespace Client.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        // Regex per a que la contrassenya compleixi les condicions de l'API mitjançaant el client
        [RegularExpression(@"^(?=.*[a-z])(?=.*\d).{6,}$",
        ErrorMessage = "Mínim 6 carácters, un dígit i una minúscula!")]
        public string Password { get; set; } = string.Empty;
    }
}