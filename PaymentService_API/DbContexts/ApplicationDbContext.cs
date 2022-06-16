using Microsoft.EntityFrameworkCore;
using PaymentService_API.Entities;

namespace PaymentService_API.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            Payments = Set<Payment>();
            PaymentMethods = Set<PaymentMethods>();
        }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
    }
}