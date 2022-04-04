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
        public static async Task CreateRoles(RoleManager<ApplicationRole> roleManager, ApplicationDbContext db){

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
                    var roles = new List<ApplicationRole>{
                        new ApplicationRole{ Name= StaticData.AdminRole },
                        new ApplicationRole{ Name= StaticData.ModeratorRole },
                        new ApplicationRole{ Name= StaticData.UserRole }
                    };

                    foreach(var role in roles){
                        await roleManager.CreateAsync(role);
                    }            
                }
            }
        }
    }
}