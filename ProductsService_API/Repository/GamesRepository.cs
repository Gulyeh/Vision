using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductsService_API.DbContexts;
using ProductsService_API.Dtos;
using ProductsService_API.Entites;
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

        public GamesRepository(ApplicationDbContext db, IMapper mapper, ICacheService cacheService, 
            IGameDataService gameDataService, ILogger<GamesRepository> logger)
        {
            this.gameDataService = gameDataService;
            this.logger = logger;
            this.db = db;
            this.mapper = mapper;
            this.cacheService = cacheService;
        }

        public async Task<ResponseDto> AddGame(AddGamesDto data, string Access_Token)
        {
            if(!await gameDataService.CheckGameExists<bool>(data.GameId, Access_Token)) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });
            
            var mapped = mapper.Map<Games>(data);
            
            await db.Games.AddAsync(mapped);
            if(await SaveChangesAsync()) {
                logger.LogInformation("Added Game with ID: {gameId} for purchase successfully", data.GameId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Game has been added successfuly" }); 
            }  

            logger.LogError("Could not add Game with ID: {gameId} for purchase", data.GameId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not add game" });
        }

        public async Task<ResponseDto> DeleteGame(Guid gameId)
        {
            var game = await db.Games.FirstOrDefaultAsync(x => x.GameId == gameId);
            if(game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });
            
            db.Games.Remove(game);

            if(await SaveChangesAsync()){
                logger.LogInformation("Deleted Game with ID: {gameId}", gameId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Game has been deleted successfuly" }); 
            } 

            logger.LogError("Could not delete Game with ID: {gameId}", gameId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not delete game" });
        }

        public async Task<ResponseDto> EditGame(GamesDto data)
        {
            var game = await db.Games.FirstOrDefaultAsync(x => x.GameId == data.GameId);
            if(game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });
           
            mapper.Map(data, game);

            if(await SaveChangesAsync()){
                logger.LogInformation("Edited Game with ID: {gameId} successfully", data.GameId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Game has been edited successfuly" }); 
            }  

            logger.LogError("Could not edit Game with ID: {gameId}", data.GameId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not edit game" });
        }

        public async Task<ResponseDto> GetGames(Guid? gameId = null)
        {
            if(gameId == null) {
                var games = await db.Games.ToListAsync();
                return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<GamesDto>(games));
            }
            else {
                var game = await db.Games.FirstOrDefaultAsync(x => x.Id == gameId);
                return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<GamesDto>(game));
            }           
        }

        private async Task<bool> SaveChangesAsync(){
            if(await db.SaveChangesAsync() > 0) return true;
            return false;
        }
    }
}