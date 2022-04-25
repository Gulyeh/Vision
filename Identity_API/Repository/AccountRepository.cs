using AutoMapper;
using Identity_API.Builders;
using Identity_API.DbContexts;
using Identity_API.Dtos;
using Identity_API.Extensions;
using Identity_API.Helpers;
using Identity_API.Processors;
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
        private readonly ILogger<AccountRepository> logger;

        public AccountRepository(IMapper mapper, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, ITokenService tokenService,
            ApplicationDbContext db, IRabbitMQSender rabbitMQSender, ILogger<AccountRepository> logger)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.tokenService = tokenService;
            this.db = db;
            this.rabbitMQSender = rabbitMQSender;
            this.logger = logger;
        }

        public async Task<ResponseDto> ConfirmEmail(Guid userId, string token)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User does not exist" });

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) {
                logger.LogError("Could not confirm email of User with ID: {Id}", userId); 
                return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not confirm email address" });
            }

            logger.LogInformation("User with ID: {Id} has confirmed email successfully", userId);
            return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Email has been confirmed" });
        }

        public async Task<ResponseDto> Login(LoginDto loginData)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == loginData.Email);
            if (user is null) return new ResponseDto(false, StatusCodes.Status401Unauthorized, new[] { "Wrong email or password" });

            var results = await signInManager.CheckPasswordSignInAsync(user, loginData.Password, false);
            if (!results.Succeeded) return new ResponseDto(false, StatusCodes.Status401Unauthorized, new[] { "Wrong email or password" });

            var isBanned = await db.BannedUsers.FirstOrDefaultAsync(x => x.UserId == user.Id && x.BanExpires > DateTime.Now);
            if (isBanned is not null) 
            {
                logger.LogInformation("User with ID: {Id} is banned from logging in", user.Id); 
                return new ResponseDto(false, StatusCodes.Status401Unauthorized, new
                {
                    Message = "Your account has been banned",
                    Reason = isBanned.Reason,
                    BanExpires = isBanned.BanExpires.ToShortDate(),
                });
            }

            var userDtoBuilder = new UserDtoBuilder(tokenService, user);
            userDtoBuilder.SetEmail();
            await userDtoBuilder.SetToken();
            var userDto = userDtoBuilder.Build();

            logger.LogInformation("User with ID: {Id} logged in successfully", user.Id); 
            return new ResponseDto(true, StatusCodes.Status200OK, userDto);
        }

        public async Task<ResponseDto> Register(RegisterDto registerData, string baseUri)
        {
            var userExists = await userManager.Users.AnyAsync(x => x.Email == registerData.Email);
            if (userExists) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User with this email already exists" });        

            var user = mapper.Map<ApplicationUser>(registerData);
            user.UserName = registerData.Email;

            var result = await userManager.CreateAsync(user, registerData.Password);
            if (!result.Succeeded) {
                logger.LogError("Could not register a User with Email: {email}", registerData.Email); 
                return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not register user" });
            }

            await userManager.AddToRoleAsync(user, StaticData.UserRole);
            rabbitMQSender.SendMessage(new { userId = user.Id }, "CreateUserQueue");

            var email = new EmailProcessor(userManager, user).GenerateEmail(EmailTypes.Confirmation);
            await email.GenerateEmailData(baseUri);
            var emailDto = email.Build();

            rabbitMQSender.SendMessage(emailDto, "SendEmailQueue");
            logger.LogInformation("User with Email: {email} registered successfully", registerData.Email); 
            return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Account has been registered successfuly. Please check your mailbox and confirm your email address" });
        }

        public async Task<ResponseDto> RequestResetPassword(string Email, string baseUri)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Email == Email);
            if (user is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User with this email does not exist" });

            var email = new EmailProcessor(userManager, user).GenerateEmail(EmailTypes.ResetPassword);
            await email.GenerateEmailData(baseUri);
            var emailDto = email.Build();

            rabbitMQSender.SendMessage(emailDto, "SendEmailQueue");

            logger.LogInformation("User with Email: {email} requested reset password", Email); 
            return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Please check your inbox to your reset password" });
        }

        public async Task<ResponseDto> ResetPassword(ResetPasswordData data)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == data.userId);
            if (user is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User does not exist" });
            
            var result = await userManager.ResetPasswordAsync(user, data.Token, data.NewPassword);
            if(!result.Succeeded) {
                logger.LogError("Could not reset password for User with ID: {userId}", data.userId); 
                return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not reset password" });
            }

            logger.LogInformation("Resetted password for User with ID: {userId}", data.userId); 
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Password has been set successfully" });
        }
    }
}