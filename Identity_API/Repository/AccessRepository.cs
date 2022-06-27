using AutoMapper;
using Identity_API.DbContexts;
using Identity_API.Dtos;
using Identity_API.Entities;
using Identity_API.Helpers;
using Identity_API.RabbitMQSender;
using Identity_API.Repository.IRepository;
using Identity_API.Statics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using static Identity_API.Statics.StaticData;

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
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IRabbitMQSender rabbitMQSender;

        public AccessRepository(IMapper mapper, UserManager<ApplicationUser> userManager, ApplicationDbContext db,
                    ILogger<AccessRepository> logger, IMemoryCache memoryCache, IOptions<ServersData> options, RoleManager<ApplicationRole> roleManager, IRabbitMQSender rabbitMQSender)
        {
            this.memoryCache = memoryCache;
            this.options = options;
            this.roleManager = roleManager;
            this.rabbitMQSender = rabbitMQSender;
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

        public async Task<IEnumerable<string>> GetRoles() {
            var roles = await roleManager.Roles.ToListAsync();
            return roles.Where(x => !x.Name.Equals(StaticData.OwnerRole)).Select(x => x.Name);
        }

        public async Task<ResponseDto> ChangeUserRole(Guid userId, string role, Guid requesterId)
        {
            if(userId.Equals(requesterId)) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "You cannot change your role" });

            var roleData = await roleManager.FindByNameAsync(role);
            if(roleData is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Role does not exist" });

            var user = await userManager.FindByIdAsync(userId.ToString());
            var requester = await userManager.FindByIdAsync(requesterId.ToString());
            if(user is null || requester is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User does not exist" });
            if(await userManager.IsInRoleAsync(user, roleData.Name)) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User already has this role" });

            var requesterRole = await userManager.GetRolesAsync(requester);
            var actualUserRole = await userManager.GetRolesAsync(user);
        
            if(actualUserRole.Any() && requesterRole.Any()){
                var requesterRoleValue = (int)Enum.Parse(typeof(RoleValue), requesterRole.First());
                var userRoleValue = (int)Enum.Parse(typeof(RoleValue), actualUserRole.First());
                var requestedRoleValue = (int)Enum.Parse(typeof(RoleValue), roleData.Name);

                if(requesterRoleValue > userRoleValue && requestedRoleValue <= requesterRoleValue){
                    await userManager.RemoveFromRoleAsync(user, actualUserRole.First());
                    await userManager.AddToRoleAsync(user, roleData.Name);

                    rabbitMQSender.SendMessage(userId, "KickUserQueue");
                    logger.LogInformation($"{userId} role has been changed from {actualUserRole.First()} to {roleData.Name}");
                    return new ResponseDto(true, StatusCodes.Status200OK, new[] { $"User's role has been changed from {actualUserRole.First()} to {roleData.Name}" });              
                }
            }

            logger.LogError($"{userId} role could not be changed");
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User's role could not be changed" });
        }
    }
}