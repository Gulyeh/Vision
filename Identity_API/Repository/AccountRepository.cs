using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HashidsNet;
using Identity_API.DbContexts;
using Identity_API.Dtos;
using Identity_API.Entities;
using Identity_API.Repository.IRepository;
using Identity_API.Services.IServices;
using Identity_API.Statics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity_API.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ITokenService tokenService;
        private readonly ApplicationDbContext db;

        public AccountRepository(IMapper mapper, UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService,
            ApplicationDbContext db)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.tokenService = tokenService;
            this.db = db;
        }

        public async Task<ResponseDto> BanUser(BannedUsersDto data)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == data.UserId);
            if(user is null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User does not exist" });

            var alreadyBanned = await db.BannedUsers.FirstOrDefaultAsync(x => x.UserId == data.UserId);
            if(alreadyBanned is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User has beed already banned" });

            var mapped = mapper.Map<BannedUsers>(data);
            await db.BannedUsers.AddAsync(mapped);
            if(await db.SaveChangesAsync() > 0) return new ResponseDto(true, StatusCodes.Status200OK, new[] { "User has been banned successfuly" });

            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not ban a user" });
        }

        public async Task<ResponseDto> ConfirmEmail(string userId, string token)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if(user is null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User does not exist" });

            var result = await userManager.ConfirmEmailAsync(user, token);
            if(!result.Succeeded) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not confirm email address" });

            return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Email has been confirmed" });
        }

        public async Task<ResponseDto> Login(LoginDto loginData)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == loginData.Email);
            if(user is null) return new ResponseDto(false, StatusCodes.Status401Unauthorized, new[] { "Wrong email or password" });

            var results = await signInManager.PasswordSignInAsync(user, loginData.Password, false, false);
            if(!results.Succeeded) return new ResponseDto(false, StatusCodes.Status401Unauthorized, new[] { "Wrong email or password" });

            return new ResponseDto(true, StatusCodes.Status200OK, new UserDto(
                username: string.Empty,
                email: user.Email,
                token: await tokenService.CreateToken(user),
                photoUrl: string.Empty,
                description: string.Empty
            ));
        }

        public async Task<ResponseDto> Register(RegisterDto registerData, string baseUri)
        {
            if(await UserExists(registerData.Email)){
                return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User with this email already exists" });
            }

            var user = mapper.Map<IdentityUser>(registerData);
            user.UserName = registerData.Email;

            var result = await userManager.CreateAsync(user, registerData.Password);
            if(!result.Succeeded) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not register user" });
            await userManager.AddToRoleAsync(user, StaticData.UserRole);

            var token = Encoding.UTF8.GetBytes(await userManager.GenerateEmailConfirmationTokenAsync(user));

            //TODO:SMTP Server with Confirmation Email

            return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Account has been registered successfuly. Please check your mailbox and confirm your email address" });
        }

        public async Task<ResponseDto> SingOut()
        {
            await signInManager.SignOutAsync();
            return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Signed out successfuly" });
        }

        public async Task<ResponseDto> UnbanUser(string userId)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if(user is null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User does not exist" });

            var alreadyBanned = await db.BannedUsers.FirstOrDefaultAsync(x => x.UserId == userId);
            if(alreadyBanned is null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User is not banned" });

            db.BannedUsers.Remove(alreadyBanned);
            if(await db.SaveChangesAsync() > 0) return new ResponseDto(true, StatusCodes.Status200OK, new[] { "User has been unbanned successfuly" });

            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not unban a user" });
        }

        private async Task<bool> UserExists(string email){
            return await userManager.Users.AnyAsync(e => e.Email == email);
        }
    }
}