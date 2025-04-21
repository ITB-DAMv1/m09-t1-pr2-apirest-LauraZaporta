using Microsoft.AspNetCore.Identity;

namespace Client.DTOs
{
    public class UserBD : IdentityUser
    {
        public IList<GameGetDTO> VotedGames { get; set; } = new List<GameGetDTO>();
    }
}