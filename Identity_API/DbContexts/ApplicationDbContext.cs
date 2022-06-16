using Identity_API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity_API.DbContexts
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public DateTime SentConfirmationDate { get; set; }
    }

    public class ApplicationRole : IdentityRole<Guid> { }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            BannedUsers = Set<BannedUsers>();
        }

        public DbSet<BannedUsers> BannedUsers { get; set; }
    }
}