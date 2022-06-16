using AutoMapper;
using Identity_API.DbContexts;
using Identity_API.Dtos;
using Identity_API.Entities;
using Identity_API.Helpers;
using Identity_API.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Identity_API.Repository
{
    public class AccessRepository : IAccessRepository
    {
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext db;
        private readonly ILogger<AccessRepository> logger;
        private readonly IMemoryCache memoryCache;
        private readonly IOptions<ServersData> options;

        public AccessRepository(IMapper mapper, UserManager<ApplicationUser> userManager, ApplicationDbContext db,
                    ILogger<AccessRepository> logger, IMemoryCache memoryCache, IOptions<ServersData> options)
        {
            this.memoryCache = memoryCache;
            this.options = options;
            this.mapper = mapper;
            this.userManager = userManager;
            this.db = db;
            this.logger = logger;
        }

        public async Task<ResponseDto> BanUser(BannedUsersDto data)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == data.UserId);
            if (user is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User does not exist" });

            var alreadyBanned = await db.BannedUsers.FirstOrDefaultAsync(x => x.UserId == data.UserId);
            if (alreadyBanned is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User has beed already banned" });

            var mapped = mapper.Map<BannedUsers>(data);
            await db.BannedUsers.AddAsync(mapped);
            if (await db.SaveChangesAsync() > 0)
            {
                logger.LogInformation("User with ID: {Id} has been banned successfully", data.UserId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "User has been banned successfully" });
            }

            logger.LogError("Could not ban User with ID: {Id}", data.UserId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not ban a user" });
        }

        public async Task<ResponseDto> UnbanUser(Guid userId)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User does not exist" });

            var alreadyBanned = await db.BannedUsers.FirstOrDefaultAsync(x => x.UserId == userId);
            if (alreadyBanned is null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User is not banned" });

            db.BannedUsers.Remove(alreadyBanned);
            if (await db.SaveChangesAsync() > 0)
            {
                logger.LogInformation("User with ID: {Id} has been unbanned successfully", userId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "User has been unbanned successfully" });
            }

            logger.LogError("Could not unban User with ID: {Id}", userId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not unban a user" });
        }

        public Task<ResponseDto> GetServerData(Guid sessionToken, Guid userId)
        {
            memoryCache.TryGetValue(userId, out Guid value);
            if (value == Guid.Empty || value != sessionToken) return Task.FromResult(new ResponseDto(false, StatusCodes.Status401Unauthorized, new[] { "Wrong sessionId provided" }));
            memoryCache.Remove(userId);

            return Task.FromResult(new ResponseDto(true, StatusCodes.Status200OK, options.Value.GetServerData()));
        }
    }
}