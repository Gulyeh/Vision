using Microsoft.EntityFrameworkCore;
using ProductsService_API.DbContexts;
using ProductsService_API.Entites;
using ProductsService_API.Services.IServices;

namespace ProductsService_API.Helpers
{
    public interface IGetCachedGames
    {
        Task<List<Games>> GetGames();
    }

    public class GetCachedGames : IGetCachedGames
    {
        private readonly ApplicationDbContext db;
        private readonly ICacheService cacheService;

        public GetCachedGames(ApplicationDbContext db, ICacheService cacheService)
        {
            this.cacheService = cacheService;
            this.db = db;
        }

        public async Task<List<Games>> GetGames()
        {
            var getGamesCache = await cacheService.TryGetFromCache<Games>(CacheType.Games);

            if (getGamesCache.Count() == 0)
            {
                var games = await db.Games.Include(x => x.Products).ToListAsync();
                foreach (var game in games) await cacheService.TryAddToCache<Games>(CacheType.Games, game);
                getGamesCache = games;
            }

            return getGamesCache;
        }
    }
}