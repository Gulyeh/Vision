using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity_API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity_API.DbContexts
{
    public class ApplicationUser : IdentityUser<Guid>{}
    public class ApplicationRole : IdentityRole<Guid>{}

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            BannedUsers = Set<BannedUsers>();
        }

        public DbSet<BannedUsers> BannedUsers { get; set; }
    }
}