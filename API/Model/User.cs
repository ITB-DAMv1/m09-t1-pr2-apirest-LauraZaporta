using Microsoft.AspNetCore.Identity;

namespace API.Model
{
    public class User : IdentityUser
    {
        public IList<Game> VotedGames { get; set; } = new List<Game>();
    }
}