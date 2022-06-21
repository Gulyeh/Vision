using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        private readonly IGameDataService gameDataService;
        private readonly ILogger<GamesRepository> logger;
        private readonly IGetCachedGames getCachedGames;
        private readonly IGameAccessService gameAccessService;

        public GamesRepository(ApplicationDbContext db, IMapper mapper, ICacheService cacheService,
            IGameDataService gameDataService, ILogger<GamesRepository> logger, IGetCachedGames getCachedGames, IGameAccessService gameAccessService)
        {
            this.getCachedGames = getCachedGames;
            this.gameAccessService = gameAccessService;
            this.gameDataService = gameDataService;
            this.logger = logger;
            this.db = db;
            this.mapper = mapper;
            this.cacheService = cacheService;
        }

        public async Task<ResponseDto> GetGame(Guid gameId, string Access_Token)
        {
            var cacheGame = await getCachedGames.GetGames();
            var gameFound = cacheGame.FirstOrDefault(x => x.GameId == gameId);
            if (gameFound is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Not found game with provided ID" });

            var mapped = mapper.Map<GamesDto>(gameFound);

            var response = await gameAccessService.CheckGameAccess(gameId, Access_Token);
            if (response is null) return new ResponseDto(false, StatusCodes.Status500InternalServerError, string.Empty);

            var responseString = response.Response.ToString();
            if (string.IsNullOrEmpty(responseString)) return new ResponseDto(false, StatusCodes.Status500InternalServerError, string.Empty);
            else
            {
                var json = JsonConvert.DeserializeObject<HasAccess>(responseString);
                if (json is null) mapped.IsPurchased = false;
                else mapped.IsPurchased = json.hasAccess;
            }

            return new ResponseDto(true, StatusCodes.Status200OK, mapped);
        }

        public async Task AddGame(NewProductDto data)
        {
            var gameExists = await db.Games.FirstOrDefaultAsync(x => x.GameId == data.GameId);
            if (gameExists is not null) return;

            var mapped = mapper.Map<Games>(data);

            await db.Games.AddAsync(mapped);
            if (await SaveChangesAsync()) {
                logger.LogInformation("Added Game with ID: {gameId} for purchase successfully", data.GameId);
                return;
            }
            logger.LogError("Could not add Game with ID: {gameId} for purchase", data.GameId);
        }

        public async Task DeleteGame(Guid gameId)
        {
            var game = await db.Games.FirstOrDefaultAsync(x => x.GameId == gameId);
            if (game is null) return;

            db.Games.Remove(game);

            if (await SaveChangesAsync()) {
                logger.LogInformation("Deleted Game with ID: {gameId}", gameId);
                return;  
            }                
            logger.LogError("Could not delete Game with ID: {gameId}", gameId);
        }

        public async Task UpdateGameData(GameProductData data){
            var game = await db.Games.FirstOrDefaultAsync(x => x.GameId == data.GameId);
            if (game is null) return;

            game.PhotoUrl = data.PhotoUrl;
            game.PhotoId = data.PhotoId;
            game.Title = data.Name;
            await SaveChangesAsync();
        }

        public async Task<ResponseDto> EditGame(GamesDto data)
        {
            var game = await db.Games.FirstOrDefaultAsync(x => x.GameId == data.GameId);
            if (game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });

            mapper.Map(data, game);

            if (await SaveChangesAsync())
            {
                logger.LogInformation("Edited Game with ID: {gameId} successfully", data.GameId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Game has been edited successfuly" });
            }

            logger.LogError("Could not edit Game with ID: {gameId}", data.GameId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not edit game" });
        }

        private async Task<bool> SaveChangesAsync()
        {
            if (await db.SaveChangesAsync() > 0) return true;
            return false;
        }
    }
}