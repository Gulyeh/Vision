using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Identity_API.DbContexts;
using Identity_API.Dtos;
using Identity_API.Entities;
using Identity_API.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity_API.Repository
{
    public class AccessRepository : IAccessRepository
    {
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext db;

        public AccessRepository(IMapper mapper, UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.db = db;
        }

        public async Task<ResponseDto> BanUser(BannedUsersDto data)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == data.UserId);
            if(user is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User does not exist" });

            var alreadyBanned = await db.BannedUsers.FirstOrDefaultAsync(x => x.UserId == data.UserId);
            if(alreadyBanned is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User has beed already banned" });

            var mapped = mapper.Map<BannedUsers>(data);
            await db.BannedUsers.AddAsync(mapped);
            if(await db.SaveChangesAsync() > 0) return new ResponseDto(true, StatusCodes.Status200OK, new[] { "User has been banned successfuly" });

            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not ban a user" });
        }

        public async Task<ResponseDto> UnbanUser(Guid userId)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if(user is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User does not exist" });

            var alreadyBanned = await db.BannedUsers.FirstOrDefaultAsync(x => x.UserId == userId);
            if(alreadyBanned is null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User is not banned" });

            db.BannedUsers.Remove(alreadyBanned);
            if(await db.SaveChangesAsync() > 0) return new ResponseDto(true, StatusCodes.Status200OK, new[] { "User has been unbanned successfuly" });

            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not unban a user" });
        }

    }
}