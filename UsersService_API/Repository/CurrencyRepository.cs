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

        public CurrencyRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<bool> ChangeFunds(CurrencyDto data)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == data.UserId);
            if(user is null) return false;
            user.CurrencyValue += data.Amount;
            if(await db.SaveChangesAsync() > 0) return true;
            return false; 
        }
    }
}