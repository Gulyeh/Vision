using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameAccessService_API.DbContexts;
using GameAccessService_API.Dtos;
using GameAccessService_API.Entites;
using GameAccessService_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace GameAccessService_API.Repository
{
    public class AccessRepository : IAccessRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;

        public AccessRepository(ApplicationDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<ResponseDto> BanUserAccess(UserAccessDto data)
        {
            var isBanned = await db.UsersGameAccess.FirstOrDefaultAsync(u => u.UserId == data.UserId && u.GameId == data.GameId && u.ExpireDate > DateTime.Now);
            if(isBanned is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User is already banned on this game" });
            var mapped = mapper.Map<UserAccess>(data);
            var result = await db.UsersGameAccess.AddAsync(mapped);
            if(await db.SaveChangesAsync() > 0) return new ResponseDto(true, StatusCodes.Status200OK, new[] { "User has been banned successfuly" });
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not ban a user" });
        }

        public async Task<bool> CheckUserAccess(string gameId, string userId)
        {
            var results = await db.UsersGameAccess.FirstOrDefaultAsync(u => u.UserId == userId && u.GameId == gameId);
            if(results is null) return true;
            return false;
        }

        public async Task<ResponseDto> UnbanUserAccess(AccessDataDto data)
        {
            var banData = await db.UsersGameAccess.FirstOrDefaultAsync(x => x.UserId == data.UserId && x.GameId == data.GameId);
            if(banData is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User is not banned in this game" });
            db.UsersGameAccess.Remove(banData);
            if(await db.SaveChangesAsync() > 0) return new ResponseDto(true, StatusCodes.Status200OK, new[] { "User has been unbanned successfuly" });
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not unban a user" });
        }
    }
}