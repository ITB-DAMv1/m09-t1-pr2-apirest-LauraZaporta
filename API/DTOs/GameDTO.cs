using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class GameDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string TeamName { get; set; }
        public IFormFile? Image { get; set; }
    }
}