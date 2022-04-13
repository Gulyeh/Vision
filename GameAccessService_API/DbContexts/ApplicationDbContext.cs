using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameAccessService_API.Entites;
using Microsoft.EntityFrameworkCore;

namespace GameAccessService_API.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            UsersGameAccess = Set<UserAccess>();
            UsersGames = Set<UserGames>();
            UsersProducts = Set<UserProducts>();
        }

        public DbSet<UserAccess> UsersGameAccess { get; set; }
        public DbSet<UserGames> UsersGames { get; set; }
        public DbSet<UserProducts> UsersProducts { get; set; }
    }
}