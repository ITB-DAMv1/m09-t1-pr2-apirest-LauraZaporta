using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class GameUpdateDTO
    {
        [Required]
        public string GameToUpdate { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? TeamName { get; set; }
        public IFormFile? Image { get; set; }
    }
}