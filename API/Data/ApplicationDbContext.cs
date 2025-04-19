using API.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApplicationDbContext : IdentityDbContext<User> // Fa una taula d'usuaris sense necessitat d'un DbSet
    {
        public DbSet<Game> Games { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            string connString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connString);
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Game>()
                .HasMany(g => g.UsersWhoVoted)
                .WithMany(u => u.VotedGames);
            modelBuilder.Entity<User>()
                .HasMany(u => u.VotedGames)
                .WithMany(g => g.UsersWhoVoted);
        }
    }
}