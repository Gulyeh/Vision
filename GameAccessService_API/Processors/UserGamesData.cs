using GameAccessService_API.DbContexts;
using GameAccessService_API.Entites;
using GameAccessService_API.Helpers;
using GameAccessService_API.Processors.Interfaces;
using GameAccessService_API.Services.IServices;

namespace GameAccessService_API.Processors
{
    public class UserGamesData : IProduct
    {
        private readonly ICacheService cacheService;
        private readonly ApplicationDbContext db;

        public UserGames userGames { get; private set; } = new();

        public UserGamesData(ICacheService cacheService, ApplicationDbContext db)
        {
            this.cacheService = cacheService;
            this.db = db;
        }

        public async Task AddToCache()
        {
            await cacheService.TryAddToCache<UserGames>(CacheType.OwnGame, userGames);
        }

        public void SetData(Guid userId, Guid gameId, Guid productId)
        {
            userGames.GameId = productId != Guid.Empty ? productId : Guid.Empty;
            userGames.UserId = userId;
        }

        public async Task SaveData()
        {
            await db.UsersGames.AddAsync(userGames);
        }

        public async Task<bool> OwnsProduct()
        {
            var cached = await cacheService.TryGetFromCache<UserGames>(CacheType.OwnGame, userGames.UserId);
            return cached.Any(x => x.GameId == userGames.GameId);
        }
    }
}