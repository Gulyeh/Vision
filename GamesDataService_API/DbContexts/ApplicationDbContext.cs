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
            Informations = Set<Informations>();
            Requirements = Set<Requirements>();
        }

        public DbSet<Games> Games { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Informations> Informations { get; set; }
        public DbSet<Requirements> Requirements { get; set; }
    }
}