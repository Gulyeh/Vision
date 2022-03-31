using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodesService_API.Entites;
using Microsoft.EntityFrameworkCore;

namespace CodesService_API.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            Codes = Set<Codes>();
        }

        public DbSet<Codes> Codes { get; set; }
    }
}