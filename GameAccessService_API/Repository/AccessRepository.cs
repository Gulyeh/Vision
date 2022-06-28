using AutoMapper;
using GameAccessService_API.DbContexts;
using GameAccessService_API.Dtos;
using GameAccessService_API.Entites;
using GameAccessService_API.Helpers;
using GameAccessService_API.Messages;
using GameAccessService_API.Processors;
using GameAccessService_API.Repository.IRepository;
using GameAccessService_API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace GameAccessService_API.Repository
{
    public class AccessRepository : IAccessRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly ICacheService cacheService;
        private readonly ILogger<AccessRepository> logger;
        private readonly IAddProductProcessor addProductProcessor;

        public AccessRepository(ApplicationDbContext db, IMapper mapper, ICacheService cacheService, ILogger<AccessRepository> logger, IAddProductProcessor addProductProcessor)
        {
            this.db = db;
            this.mapper = mapper;
            this.cacheService = cacheService;
            this.logger = logger;
            this.addProductProcessor = addProductProcessor;
        }

        public async Task<ResponseDto> BanUserAccess(UserAccessDto data)
        {
            var hasGame = await CheckUserHasGame(data.GameId, data.UserId);
            if(!hasGame) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User does not own this game" });

            var userAccess = await cacheService.TryGetFromCache<UserAccess>(CacheType.GameAccess, data.UserId);
            var isBanned = userAccess.FirstOrDefault(x => x.GameId == data.GameId && x.BanExpires > DateTime.UtcNow);
            if (isBanned is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User is already banned on this game" });

            var mapped = mapper.Map<UserAccess>(data);
            await db.UsersGameAccess.AddAsync(mapped);

            if (await db.SaveChangesAsync() > 0)
            {
                await cacheService.TryAddToCache<UserAccess>(CacheType.GameAccess, mapped);
                logger.LogInformation("User with ID: {userId} has been banned successfuly until: {date}", data.UserId, data.BanExpires);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "User has been banned successfuly" });
            }

            logger.LogError("Could not ban user with ID: {userId}", data.UserId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not ban a user" });
        }

        public async Task<(bool, BanModelDto?)> CheckUserAccess(Guid gameId, Guid userId)
        {
            var userHasGame = await CheckUserHasGame(gameId, userId);
            if(!userHasGame) return (false, null);

            (var hasAccess, var bannedData) = await CheckAccessCacheAndGetData<UserAccess>(gameId, userId, CacheType.GameAccess);
            if (hasAccess) return (true, null);
            
            return (false, mapper.Map<BanModelDto>(bannedData));
        }

        public async Task<bool> CheckUserHasGame(Guid gameId, Guid userId) => await CheckAccessCache<UserGames>(gameId, userId, CacheType.OwnGame);

        public async Task<bool> CheckUserHasProduct(Guid productId, Guid gameId, Guid userId)
        {
            if (!await CheckUserHasGame(gameId, userId)) return true;
            return await CheckAccessCache<UserProducts>(productId, userId, CacheType.OwnProduct);
        }

        public async Task<bool> AddProductOrGame(Guid userId, Guid gameId, Guid productId)
        {
            var productData = addProductProcessor.GenerateProduct(gameId);
            productData.SetData(userId, gameId, productId);
            if(await productData.OwnsProduct()) return false;
            await productData.SaveData();

            if (await db.SaveChangesAsync() > 0)
            {
                await productData.AddToCache();
                logger.LogInformation("Granted Game/Product Access with ID: {productId} to User with ID: {userId}",
                    productId == Guid.Empty ? gameId : productId, userId);
                return true;
            }

            logger.LogError("Could not grant access to Game/Product with ID: {productId} to User with ID: {userId}",
                productId == Guid.Empty ? gameId : productId, userId);
            return false;
        }

        public async Task<ResponseDto> UnbanUserAccess(Guid userId, Guid gameId)
        {
            var userAccess = await cacheService.TryGetFromCache<UserAccess>(CacheType.GameAccess, userId);
            var isBanned = userAccess.FirstOrDefault(x => x.GameId == gameId);
            if (isBanned is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User is not banned in this game" });

            db.UsersGameAccess.Remove(isBanned);
            if (await db.SaveChangesAsync() > 0)
            {
                var cachedData = await cacheService.TryGetFromCache<UserAccess>(CacheType.GameAccess, userId);
                if(cachedData is not null && cachedData.Any()) {
                    var bannedGame = cachedData.FirstOrDefault(x => x.GameId == gameId);
                    if(bannedGame is not null) await cacheService.TryRemoveFromCache<UserAccess>(CacheType.GameAccess, bannedGame);
                }
                logger.LogInformation("User with ID: {userId} has been unbanned successfuly", userId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "User has been unbanned successfuly" });
            }

            logger.LogError("User with ID: {userId} could not be unbanned", userId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not unban a user" });
        }

        private async Task<(bool, T?)> CheckAccessCacheAndGetData<T>(Guid Id, Guid userId, CacheType type) where T : BaseUser
        {
            var cache = await cacheService.TryGetFromCache<T>(type, userId);
            switch (type)
            {
                case CacheType.GameAccess:

                    var productsAccess = cache as IEnumerable<UserAccess>;
                    if (productsAccess is null) return (false, null);

                    var access = productsAccess.FirstOrDefault(x => x.GameId == Id && x.BanExpires > DateTime.UtcNow);
                    if (access is null)
                    {
                        logger.LogInformation("User with ID: {userId} has access to {gameId}", userId, Id);
                        return (true, null);
                    }

                    logger.LogInformation("User with ID: {userId} does not have access to {gameId}", userId, Id);
                    return (false, access as T);
                default:
                    return (false, null);
            }
        }

        private async Task<bool> CheckAccessCache<T>(Guid Id, Guid userId, CacheType type) where T : BaseUser
        {
            var cache = await cacheService.TryGetFromCache<T>(type, userId);

            switch (type)
            {
                case CacheType.GameAccess:
                    var productsAccess = cache as IEnumerable<UserAccess>;
                    if (productsAccess is null) return false;

                    var access = productsAccess.FirstOrDefault(x => x.GameId == Id);
                    if (access is null)
                    {
                        logger.LogInformation("User with ID: {userId} has access to {gameId}", userId, Id);
                        return true;
                    }

                    logger.LogInformation("User with ID: {userId} does not have access to {gameId}", userId, Id);
                    return false;
                case CacheType.OwnGame:
                    var ownGames = cache as IEnumerable<UserGames>;
                    if (ownGames is null) return false;

                    var ownResults = ownGames.FirstOrDefault(x => x.GameId == Id);
                    if (ownResults is null)
                    {
                        logger.LogInformation("User with ID: {userId} does not own Game: {Id}", userId, Id);
                        return false;
                    }

                    logger.LogInformation("User with ID: {userId} own Game: {Id}", userId, Id);
                    return true;
                case CacheType.OwnProduct:
                    var products = cache as IEnumerable<UserProducts>;
                    if (products is null) return false;

                    var productResult = products.FirstOrDefault(x => x.ProductId == Id);
                    if (productResult is null)
                    {
                        logger.LogInformation("User with ID: {userId} does not own Product: {Id}", userId, Id);
                        return false;
                    }

                    logger.LogInformation("User with ID: {userId} own Product: {Id}", userId, Id);
                    return true;
                default:
                    return false;
            }
        }

        public async Task RemoveGameAndProducts(DeleteGameDto data)
        {
            var game = await db.UsersGames.Where(x => x.GameId == data.GameId).ToListAsync();
            var gameAccess = await db.UsersGameAccess.Where(x => x.GameId == data.GameId).ToListAsync();
            var products = await db.UsersProducts.Where(x => data.ProductsId.Contains(x.ProductId)).ToListAsync();
            
            if(game is not null && game.Any()) db.UsersGames.RemoveRange(game);
            if(products is not null && products.Any()) db.UsersProducts.RemoveRange(products);
            if(gameAccess is not null && gameAccess.Any()) db.UsersGameAccess.RemoveRange(gameAccess);
        
            if(await db.SaveChangesAsync() > 0){
                await cacheService.TryUpdateCache<UserGames>(CacheType.OwnGame);
                await cacheService.TryUpdateCache<UserProducts>(CacheType.OwnProduct);
                await cacheService.TryUpdateCache<UserAccess>(CacheType.GameAccess);
            }        
        }

        public async Task RemoveProductAccess(Guid productId)
        {
            var products = await db.UsersProducts.Where(x => x.ProductId == productId).ToListAsync();
            if(products is null || !products.Any()) return;
            
            db.UsersProducts.RemoveRange(products);
            if(await db.SaveChangesAsync() > 0) await cacheService.TryUpdateCache<UserProducts>(CacheType.OwnProduct);
        }

        public async Task<bool> CheckUserIsBanned(Guid userId, Guid gameId)
        {
            var hasGame = await CheckUserHasGame(gameId, userId);
            if(!hasGame) return false;

            var userAccess = await cacheService.TryGetFromCache<UserAccess>(CacheType.GameAccess, userId);
            var isBanned = userAccess.FirstOrDefault(x => x.GameId == gameId && x.BanExpires > DateTime.UtcNow);
            if (isBanned is not null) return true;

            return false;
        }
    }
}