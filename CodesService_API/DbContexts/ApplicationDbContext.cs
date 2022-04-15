using CodesService_API.Entites;
using Microsoft.EntityFrameworkCore;

namespace CodesService_API.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            Codes = Set<Codes>();
            CodesUsed = Set<CodesUsed>();
        }

        public DbSet<Codes> Codes { get; set; }
        public DbSet<CodesUsed> CodesUsed { get; set; }
    }
}