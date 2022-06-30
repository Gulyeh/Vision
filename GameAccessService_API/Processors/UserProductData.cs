using GameAccessService_API.DbContexts;
using GameAccessService_API.Entites;
using GameAccessService_API.Helpers;
using GameAccessService_API.Processors.Interfaces;
using GameAccessService_API.Services.IServices;

namespace GameAccessService_API.Processors
{
    public class UserProductData : IProduct
    {
        private readonly ICacheService cacheService;
        private readonly ApplicationDbContext db;

        public UserProducts userProduct { get; private set; } = new();
        public Guid GameId { get; set; }
        public UserProductData(ICacheService cacheService, ApplicationDbContext db)
        {
            this.cacheService = cacheService;
            this.db = db;
        }

        public async Task AddToCache()
        {
            await cacheService.TryAddToCache<UserProducts>(CacheType.OwnProduct, userProduct);
        }

        public void SetData(Guid userId, Guid gameId, Guid productId)
        {
            userProduct.UserId = userId;
            userProduct.ProductId = productId != Guid.Empty && gameId != Guid.Empty ? productId : Guid.Empty;
            GameId = gameId;
        }

        public async Task SaveData()
        {
            await db.UsersProducts.AddAsync(userProduct);
        }

        public async Task<bool> OwnsProduct()
        {
            var cachedGames = await cacheService.TryGetFromCache<UserGames>(CacheType.OwnGame, userProduct.UserId);
            if (cachedGames.Any(x => x.GameId == GameId))
            {
                var cached = await cacheService.TryGetFromCache<UserProducts>(CacheType.OwnProduct, userProduct.UserId);
                return cached.Any(x => x.ProductId == userProduct.ProductId);
            }

            return true;
        }
    }
}