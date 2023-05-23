using Microsoft.EntityFrameworkCore;
using GameLibraryAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace GameLibraryAPI
{
    public class GameDBContext : IdentityDbContext<IdentityUser>
    {
        public GameDBContext(DbContextOptions<GameDBContext> options) : base(options)
        {
        }

        public DbSet<Game> Game { get; set; }
        public DbSet<Genre> Genre { get; set; }

    }
}
