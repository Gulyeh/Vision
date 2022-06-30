using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProdcutsService_API.RabbitMQRPC;
using ProductsService_API.DbContexts;
using ProductsService_API.Dtos;
using ProductsService_API.Entites;
using ProductsService_API.Helpers;
using ProductsService_API.Messages;
using ProductsService_API.Repository.IRepository;
using ProductsService_API.Services.IServices;

namespace ProductsService_API.Repository
{
    public class GamesRepository : IGamesRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly ICacheService cacheService;
        private readonly ILogger<GamesRepository> logger;
        private readonly IGetCachedGames getCachedGames;
        private readonly IRabbitMQRPC rabbitMQRPC;
        private readonly IUploadService uploadService;

        public GamesRepository(ApplicationDbContext db, IMapper mapper, ICacheService cacheService,
            ILogger<GamesRepository> logger, IGetCachedGames getCachedGames, IRabbitMQRPC rabbitMQRPC,
            IUploadService uploadService)
        {
            this.getCachedGames = getCachedGames;
            this.rabbitMQRPC = rabbitMQRPC;
            this.uploadService = uploadService;
            this.logger = logger;
            this.db = db;
            this.mapper = mapper;
            this.cacheService = cacheService;
        }

        public async Task<ResponseDto> GetGame(Guid gameId, Guid userId)
        {
            var cacheGame = await getCachedGames.GetGames();
            var gameFound = cacheGame.FirstOrDefault(x => x.GameId == gameId);
            if (gameFound is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Not found game with provided ID" });

            var mapped = mapper.Map<GamesDto>(gameFound);

            var response = await rabbitMQRPC.SendAsync("CheckProductAccessQueue", new CheckProductAccessDto(userId, gameId, Guid.Empty));
            if (response is null || string.IsNullOrEmpty(response)) return new ResponseDto(false, StatusCodes.Status500InternalServerError, string.Empty);

            mapped.IsPurchased = bool.Parse(response);

            return new ResponseDto(true, StatusCodes.Status200OK, mapped);
        }

        public async Task AddGame(NewProductDto data)
        {
            var gameExists = await db.Games.FirstOrDefaultAsync(x => x.GameId == data.GameId);
            if (gameExists is not null) return;

            var mapped = mapper.Map<Games>(data);

            await db.Games.AddAsync(mapped);
            if (await SaveChangesAsync())
            {
                await cacheService.TryAddToCache<Games>(CacheType.Games, mapped);
                logger.LogInformation("Added Game with ID: {gameId} for purchase successfully", data.GameId);
                return;
            }
            logger.LogError("Could not add Game with ID: {gameId} for purchase", data.GameId);
        }

        public async Task<bool> DeleteGame(Guid gameId)
        {
            var game = await db.Games.Include(x => x.Products).FirstOrDefaultAsync(x => x.GameId == gameId);
            if (game is null) return false;

            db.Games.Remove(game);

            if (await SaveChangesAsync())
            {
                foreach (var product in game.Products) await uploadService.DeletePhoto(product.PhotoId);
                var cachedGame = await FindGame(gameId);
                if (cachedGame is not null) await cacheService.DeleteFromCache<Games>(CacheType.Games, cachedGame);

                logger.LogInformation("Deleted Game with ID: {gameId}", gameId);
                return true;
            }

            logger.LogError("Could not delete Game with ID: {gameId}", gameId);
            return false;
        }

        public async Task UpdateGameData(GameProductData data)
        {
            var game = await db.Games.Include(x => x.Products).FirstOrDefaultAsync(x => x.GameId == data.GameId);
            if (game is null) return;

            game.PhotoUrl = data.PhotoUrl;
            game.PhotoId = data.PhotoId;
            game.Title = data.Name;

            if (await SaveChangesAsync()) await cacheService.TryReplaceCache<Games>(CacheType.Games, game);

        }

        public async Task<ResponseDto> EditGame(EditPackageDto data)
        {
            var game = await db.Games.Include(x => x.Products).FirstOrDefaultAsync(x => x.GameId == data.Id);
            if (game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });

            mapper.Map(data, game);

            if (await SaveChangesAsync())
            {
                logger.LogInformation("Edited Game with ID: {gameId} successfully", data.Id);
                await cacheService.TryReplaceCache<Games>(CacheType.Games, game);

                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Game has been edited successfuly" });
            }

            logger.LogError("Could not edit Game with ID: {gameId}", data.Id);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not edit game" });
        }

        private async Task<bool> SaveChangesAsync()
        {
            if (await db.SaveChangesAsync() > 0) return true;
            return false;
        }

        public async Task<Games?> FindGame(Guid gameId)
        {
            var game = await getCachedGames.GetGames();
            return game.FirstOrDefault(x => x.GameId == gameId);
        }
    }
}