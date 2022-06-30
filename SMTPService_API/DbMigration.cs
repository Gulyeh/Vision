using Microsoft.EntityFrameworkCore;
using SMTPService_API.DBContexts;

namespace SMTPService_API
{
    public static class DbMigration
    {
        public async static Task Migrate(WebApplication builder)
        {
            using var scope = builder.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await ctx.Database.MigrateAsync();
        }
    }
}