using Identity_API.Statics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity_API.DbContexts
{
    public static class RoleSeeder
    {
        public static async Task SeedRoles(WebApplication builder){
            using var scope = builder.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var role = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            await CreateRoles(role, context);
        }

        private static async Task CreateRoles(RoleManager<ApplicationRole> roleManager, ApplicationDbContext db)
        {

            if (db.Database.CanConnect())
            {
                if (db.Database.IsRelational())
                {
                    var pendingMigrations = db.Database.GetPendingMigrations();

                    if (pendingMigrations != null && pendingMigrations.Any())
                    {
                        db.Database.Migrate();
                    }
                }

                if (!await roleManager.Roles.AnyAsync())
                {
                    var roles = new List<ApplicationRole>{
                        new ApplicationRole{ Name= StaticData.AdminRole },
                        new ApplicationRole{ Name= StaticData.ModeratorRole },
                        new ApplicationRole{ Name= StaticData.UserRole }
                    };

                    foreach (var role in roles)
                    {
                        await roleManager.CreateAsync(role);
                    }
                }
            }
        }
    }
}