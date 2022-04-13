using Microsoft.EntityFrameworkCore;
using PaymentService_API.Entities;

namespace PaymentService_API.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            Payments = Set<Payment>();
        }

        public DbSet<Payment> Payments { get; set; }
    }
}