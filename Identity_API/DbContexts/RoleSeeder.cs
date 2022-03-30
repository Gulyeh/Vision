using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity_API.Statics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity_API.DbContexts
{
    public static class RoleSeeder
    {
        public static async Task CreateRoles(RoleManager<IdentityRole> roleManager, ApplicationDbContext db){

            if (db.Database.CanConnect())
            {
                if(db.Database.IsRelational())
                {
                    var pendingMigrations = db.Database.GetPendingMigrations();

                    if (pendingMigrations != null && pendingMigrations.Any())
                    {
                        db.Database.Migrate();
                    }
                }

                if(!await roleManager.Roles.AnyAsync()){
                    var roles = new List<IdentityRole>{
                        new IdentityRole{ Name= StaticData.AdminRole },
                        new IdentityRole{ Name= StaticData.ModeratorRole },
                        new IdentityRole{ Name= StaticData.UserRole }
                    };

                    foreach(var role in roles){
                        await roleManager.CreateAsync(role);
                    }            
                }
            }
        }
    }
}