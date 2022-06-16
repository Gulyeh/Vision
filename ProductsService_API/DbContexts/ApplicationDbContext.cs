using Microsoft.EntityFrameworkCore;
using ProductsService_API.Entites;

namespace ProductsService_API.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            Products = Set<Products>();
            Games = Set<Games>();
            Currencies = Set<Currency>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Games>()
               .HasMany(x => x.Products)
               .WithOne(x => x.Game)
               .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<Products> Products { get; set; }
        public DbSet<Games> Games { get; set; }
        public DbSet<Currency> Currencies { get; set; }

    }
}