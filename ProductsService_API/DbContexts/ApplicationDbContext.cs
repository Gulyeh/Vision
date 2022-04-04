using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductsService_API.Entites;

namespace ProductsService_API.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            GamesProducts = Set<Products>();
        }

        public DbSet<Products> GamesProducts { get; set; }
    }
}