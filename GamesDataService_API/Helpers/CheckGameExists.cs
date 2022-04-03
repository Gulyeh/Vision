using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamesDataService_API.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace GamesDataService_API.Helpers
{
    public class CheckGameExists
    {
        private readonly ApplicationDbContext db;

        public CheckGameExists(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<bool> GameExists(Guid gameId){
            var gameExists = await db.Games.FirstOrDefaultAsync(x => x.Id == gameId);
            if(gameExists is null) return false;
            return true;
        }
    }
}