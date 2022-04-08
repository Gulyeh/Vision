using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UsersService_API.Entites;

namespace UsersService_API.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            UsersFriends = Set<UsersFriends>();
            FriendRequests = Set<FriendRequests>();
            Users = Set<Users>();
        }

        public DbSet<UsersFriends> UsersFriends { get; set; }
        public DbSet<FriendRequests> FriendRequests { get; set; }
        public DbSet<Users> Users { get; set; }
    }
}