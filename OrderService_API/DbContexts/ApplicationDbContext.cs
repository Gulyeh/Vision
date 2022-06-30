using Microsoft.EntityFrameworkCore;
using OrderService_API.Entities;

namespace OrderService_API.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            Orders = Set<Order>();
        }

        public DbSet<Order> Orders { get; set; }
    }
}