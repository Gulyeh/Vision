using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameAccessService_API.DbContexts;
using GameAccessService_API.Processors.Interfaces;
using GameAccessService_API.Services.IServices;

namespace GameAccessService_API.Processors
{
    public class AddProductProcessor
    {
        private readonly ICacheService cacheService;
        private readonly ApplicationDbContext db;

        public AddProductProcessor(ICacheService cacheService, ApplicationDbContext db)
        {
            this.cacheService = cacheService;
            this.db = db;
        }
    
        public IProduct GenerateProduct(Guid? productId){
            return productId switch{
                null => new UserGamesData(cacheService, db),
                _ => new UserProductData(cacheService, db)
            };
        }
    }
}