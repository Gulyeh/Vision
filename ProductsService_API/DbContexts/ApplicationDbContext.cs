using Microsoft.EntityFrameworkCore;
using ProductsService_API.Entites;

namespace ProductsService_API.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            GamesProducts = Set<Products>();
            Games = Set<Games>();
        }

        public DbSet<Products> GamesProducts { get; set; }
        public DbSet<Games> Games { get; set; }
    }
}