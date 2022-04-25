using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public UserProductData(ICacheService cacheService, ApplicationDbContext db)
        {
            this.cacheService = cacheService;
            this.db = db;
        }

        public async Task AddToCache()
        {
            await cacheService.TryAddToCache<UserProducts>(CacheType.OwnProduct, userProduct);
        }

        public void SetData(Guid userId, Guid gameId, Guid? productId = null)
        {
            userProduct.GameId = gameId;
            userProduct.UserId = userId;
            userProduct.ProductId = productId != null ? (Guid)productId : Guid.Empty;
        }

        public async Task SaveData(){
            await db.UsersProducts.AddAsync(userProduct);
        }
    }
}