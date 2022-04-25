using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UsersService_API.DbContexts;
using UsersService_API.Messages;
using UsersService_API.Repository.IRepository;

namespace UsersService_API.Repository
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly ApplicationDbContext db;
        private readonly ILogger<CurrencyRepository> logger;

        public CurrencyRepository(ApplicationDbContext db, ILogger<CurrencyRepository> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        public async Task<bool> ChangeFunds(CurrencyDto data)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == data.UserId);
            if(user is null) return false;
            user.CurrencyValue += data.Amount;
            if(await db.SaveChangesAsync() > 0) {
                logger.LogInformation("Increased currency for User with ID: {userId} by {amount}", data.UserId, data.Amount);
                return true;
            }
            logger.LogError("Could not increase currency for User with ID: {userId}", data.UserId);
            return false; 
        }
    }
}