using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UsersService_API.DbContexts;

namespace UsersService_API
{
    public static class DbMigration
    {
        public async static Task Migrate(WebApplication builder){
            using var scope = builder.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();          
            using var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await ctx.Database.MigrateAsync();
        }
    }
}