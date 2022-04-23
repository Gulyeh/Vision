using AutoMapper;
using Identity_API.DbContexts;
using Identity_API.Dtos;
using Identity_API.Extensions;
using Identity_API.Helpers;
using Identity_API.RabbitMQSender;
using Identity_API.Repository.IRepository;
using Identity_API.Services.IServices;
using Identity_API.Statics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Identity_API.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ITokenService tokenService;
        private readonly ApplicationDbContext db;
        private readonly IRabbitMQSender rabbitMQSender;

        public AccountRepository(IMapper mapper, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, ITokenService tokenService,
            ApplicationDbContext db, IRabbitMQSender rabbitMQSender)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.tokenService = tokenService;
            this.db = db;
            this.rabbitMQSender = rabbitMQSender;
        }

        public async Task<ResponseDto> ConfirmEmail(Guid userId, string token)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User does not exist" });

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not confirm email address" });

            return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Email has been confirmed" });
        }

        public async Task<ResponseDto> Login(LoginDto loginData)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == loginData.Email);
            if (user is null) return new ResponseDto(false, StatusCodes.Status401Unauthorized, new[] { "Wrong email or password" });

            var results = await signInManager.CheckPasswordSignInAsync(user, loginData.Password, false);
            if (!results.Succeeded) return new ResponseDto(false, StatusCodes.Status401Unauthorized, new[] { "Wrong email or password" });

            var isBanned = await db.BannedUsers.FirstOrDefaultAsync(x => x.UserId == user.Id && x.BanExpires > DateTime.Now);
            if (isBanned is not null) return new ResponseDto(false, StatusCodes.Status401Unauthorized, new
            {
                Message = "Your account has been banned",
                Reason = isBanned.Reason,
                BanExpires = isBanned.BanExpires.ToShortDate(),
            });

            var userDto = new UserDto();
            userDto.Email = user.Email;
            userDto.Token = await tokenService.CreateToken(user);

            return new ResponseDto(true, StatusCodes.Status200OK, userDto);
        }

        public async Task<ResponseDto> Register(RegisterDto registerData, string baseUri)
        {
            if (await UserExists(registerData.Email))
            {
                return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User with this email already exists" });
            }

            var user = mapper.Map<ApplicationUser>(registerData);
            user.UserName = registerData.Email;

            var result = await userManager.CreateAsync(user, registerData.Password);
            if (!result.Succeeded) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not register user" });
            await userManager.AddToRoleAsync(user, StaticData.UserRole);

            var token = Encoding.UTF8.GetBytes(await userManager.GenerateEmailConfirmationTokenAsync(user));
            var link = $"{baseUri}/ConfirmEmail?userId={user.Id}&token={token}";

            rabbitMQSender.SendMessage(new { userId = user.Id }, "CreateUserQueue");

            var emailDto = new EmailDataDto();
            emailDto.Content = $"Confirm your email by entering {link}";
            emailDto.EmailType = EmailTypes.Confirmation;
            emailDto.ReceiverEmail = user.UserName;
            emailDto.userId = user.Id;

            rabbitMQSender.SendMessage(emailDto, "SendEmailQueue");

            return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Account has been registered successfuly. Please check your mailbox and confirm your email address" });
        }

        private async Task<bool> UserExists(string email)
        {
            return await userManager.Users.AnyAsync(e => e.Email == email);
        }
    }
}