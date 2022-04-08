using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameAccessService_API.DbContexts;
using GameAccessService_API.Dtos;
using GameAccessService_API.Entites;
using GameAccessService_API.Helpers;
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

        public AccessRepository(ApplicationDbContext db, IMapper mapper, ICacheService cacheService)
        {
            this.db = db;
            this.mapper = mapper;
            this.cacheService = cacheService;
        }

        public async Task<ResponseDto> BanUserAccess(UserAccessDto data)
        {
            var isBanned = await db.UsersGameAccess.FirstOrDefaultAsync(u => u.UserId == data.UserId && u.GameId == data.GameId && u.ExpireDate > DateTime.Now);
            if(isBanned is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User is already banned on this game" });
            var mapped = mapper.Map<UserAccess>(data);
            var result = await db.UsersGameAccess.AddAsync(mapped);
            if(await db.SaveChangesAsync() > 0){
                await cacheService.TryAddToCache<UserAccess>(CacheType.GameAccess, mapped);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "User has been banned successfuly" });
            }
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not ban a user" });
        }

        public async Task<bool> CheckUserAccess(Guid gameId, Guid userId)
        {
            return await CheckCache<UserAccess>(gameId, userId, CacheType.GameAccess);
        }

        public async Task<bool> CheckUserHasGame(Guid gameId, Guid userId)
        {
            return await CheckCache<UserGames>(gameId, userId, CacheType.OwnGame);
        }

        public async Task<ResponseDto> UnbanUserAccess(AccessDataDto data)
        {
            var isBanned = await db.UsersGameAccess.FirstOrDefaultAsync(x => x.UserId == data.UserId && x.GameId == data.GameId);
            if(isBanned is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User is not banned in this game" });
            db.UsersGameAccess.Remove(isBanned);
            if(await db.SaveChangesAsync() > 0){
                 await cacheService.TryRemoveFromCache<UserAccess>(CacheType.GameAccess, isBanned);
                 return new ResponseDto(true, StatusCodes.Status200OK, new[] { "User has been unbanned successfuly" });
            }
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not unban a user" });
        }

        private async Task<bool> CheckCache<T>(Guid gameId, Guid userId, CacheType type) where T : BaseUser
        {
            var cache = await cacheService.TryGetFromCache<T>(type);
            var results = cache.FirstOrDefault(x => x.GameId == gameId && x.UserId == userId);

            switch(type){
                case CacheType.GameAccess:
                    if(results is null) return true;
                    return false;
                case CacheType.OwnGame:
                    if(results is null) return false;
                    return true;
                default:
                    return false;
            }
        }
    }
}