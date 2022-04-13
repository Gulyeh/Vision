using GamesDataService_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace GamesDataService_API.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            Games = Set<Games>();
            News = Set<News>();
        }

        public DbSet<Games> Games { get; set; }
        public DbSet<News> News { get; set; }
    }
}