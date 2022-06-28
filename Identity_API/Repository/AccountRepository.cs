using AutoMapper;
using Identity_API.Builders;
using Identity_API.DbContexts;
using Identity_API.Dtos;
using Identity_API.Helpers;
using Identity_API.Processors;
using Identity_API.RabbitMQSender;
using Identity_API.Repository.IRepository;
using Identity_API.Services.IServices;
using Identity_API.Statics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Web;

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
        private readonly UrlEncoder urlEncoder;
        private readonly IMemoryCache memoryCache;
        private readonly IEmailProcessor emailProcessor;
        private readonly string baseUri;

        public AccountRepository(IMapper mapper, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, ITokenService tokenService,
            ApplicationDbContext db, IRabbitMQSender rabbitMQSender, ILogger<AccountRepository> logger, UrlEncoder urlEncoder, IMemoryCache memoryCache, IEmailProcessor emailProcessor,
            IConfiguration config)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.tokenService = tokenService;
            this.db = db;
            this.rabbitMQSender = rabbitMQSender;
            this.logger = logger;
            this.urlEncoder = urlEncoder;
            this.memoryCache = memoryCache;
            this.emailProcessor = emailProcessor;
            baseUri = config["WebClientAddress"];
        }

        public async Task<ResponseDto> Toggle2FA(Guid userId, string code)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User does not exist" });

            bool verifyToken = await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, code);
            if (!verifyToken) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Code is wrong" });

            var isEnabled = user.TwoFactorEnabled ? false : true;
            var result = await userManager.SetTwoFactorEnabledAsync(user, isEnabled);
            if (!result.Succeeded) return new ResponseDto(false, StatusCodes.Status500InternalServerError, new[] { "Something went wrong with your request" });

            var text = isEnabled ? "enabled" : "disabled";
            return new ResponseDto(true, StatusCodes.Status200OK, new[] { $"Two-factor authentication has been {text}", });
        }

        public async Task<ResponseDto> Generate2FA(Guid userId)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User does not exist" });

            if (user.TwoFactorEnabled) return new ResponseDto(false, StatusCodes.Status200OK, new[] { "Two-factor Authentication is activated" });

            var tokenData = await userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(tokenData))
            {
                await userManager.ResetAuthenticatorKeyAsync(user);
                tokenData = await userManager.GetAuthenticatorKeyAsync(user);
            }

            var qrCode = string.Format(
                CultureInfo.InvariantCulture,
                "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
                urlEncoder.Encode("Vision"),
                urlEncoder.Encode(user.Email),
                urlEncoder.Encode(tokenData));
            return new ResponseDto(true, StatusCodes.Status200OK, new { TokenCode = tokenData, QrCodeUri = qrCode });
        }

        public async Task<ResponseDto> ChangePassword(PasswordDataDto data)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == data.userId);
            if (user is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User does not exist" });

            var isNewPasswordTheSameAsCurrent = await signInManager.UserManager.CheckPasswordAsync(user, data.NewPassword);
            if (isNewPasswordTheSameAsCurrent) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "New password cannot be the same as current one" });

            var isOldPasswordCorrect = await signInManager.UserManager.CheckPasswordAsync(user, data.CurrentPassword);
            if (!isOldPasswordCorrect) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Old password is wrong" });

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, data.NewPassword);
            if (!result.Succeeded)
            {
                logger.LogError("Could not change password for user with ID: {id}", data.userId);
                return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Cannot set new password" });
            }

            logger.LogInformation("User with ID: {id} has changed password", data.userId);
            return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Password has been changed successfully" });
        }

        public async Task<ResponseDto> ConfirmEmail(Guid userId, string token)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User does not exist" });
            if (user.EmailConfirmed) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Email has been already confirmed" });

            var decodedToken = HttpUtility.UrlDecode(token).Replace(" ", "+");
            var result = await userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
            {
                logger.LogError("Could not confirm email of User with ID: {Id} and Token: {token}", userId, decodedToken);
                return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not confirm email address" });
            }

            logger.LogInformation("User with ID: {Id} has confirmed email successfully", userId);
            return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Email has been confirmed" });
        }

        public async Task<ResponseDto> Login(LoginDto loginData)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == loginData.Email);
            if (user is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Wrong email or password" });

            var results = await signInManager.CheckPasswordSignInAsync(user, loginData.Password, false);
            if (!results.Succeeded) return new ResponseDto(false, StatusCodes.Status401Unauthorized, new[] { "Wrong email or password" });

            var isBanned = await db.BannedUsers.FirstOrDefaultAsync(x => x.UserId == user.Id && x.BanExpires > DateTime.UtcNow);
            if (isBanned is not null)
            {
                logger.LogInformation("User with ID: {Id} is banned from logging in", user.Id);
                return new ResponseDto(false, StatusCodes.Status403Forbidden, new
                {
                    Reason = isBanned.Reason,
                    BanExpires = isBanned.BanExpires,
                    BanDate = isBanned.BanDate
                });
            }

            if (user.TwoFactorEnabled && string.IsNullOrEmpty(loginData.AuthCode)) return new ResponseDto(false, StatusCodes.Status200OK, new { IsTFAEnabled = true });
            else if (user.TwoFactorEnabled && !string.IsNullOrEmpty(loginData.AuthCode))
            {
                var TFAResults = await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, loginData.AuthCode);
                if (!TFAResults)
                {
                    return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Provided code is wrong" });
                }
            }

            var userDtoBuilder = new UserDtoBuilder(tokenService, user);
            userDtoBuilder.SetEmail();
            userDtoBuilder.SetSessionId();
            await userDtoBuilder.SetToken();
            var userDto = userDtoBuilder.Build();

            memoryCache.Set(user.Id, userDto.SessionId);
            logger.LogInformation("User with ID: {Id} logged in successfully", user.Id);
            return new ResponseDto(true, StatusCodes.Status200OK, userDto);
        }

        public async Task<ResponseDto> Register(RegisterDto registerData)
        {
            var userExists = await userManager.Users.AnyAsync(x => x.Email == registerData.Email);
            if (userExists) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User with this email already exists" });

            var user = mapper.Map<ApplicationUser>(registerData);
            user.UserName = registerData.Email;

            var result = await userManager.CreateAsync(user, registerData.Password);
            if (!result.Succeeded)
            {
                logger.LogError("Could not register a User with Email: {email}", registerData.Email);
                return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not register user" });
            }

            await userManager.AddToRoleAsync(user, StaticData.UserRole);
            rabbitMQSender.SendMessage(new { userId = user.Id }, "CreateUserQueue");

            await SendConfirmationEmail(user);

            logger.LogInformation("User with Email: {email} registered successfully", registerData.Email);
            return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Account has been registered successfuly. Please check your mailbox and confirm your email address" });
        }

        public async Task<ResponseDto> RequestResetPassword(string Email)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Email == Email);
            if (user is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User with this email does not exist" });

            var email = emailProcessor.GenerateEmail(EmailTypes.ResetPassword, user);
            await email.GenerateEmailData(baseUri);
            var emailDto = email.Build();

            rabbitMQSender.SendMessage(emailDto, "SendEmailQueue");

            logger.LogInformation("User with Email: {email} requested reset password", Email);
            return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Please check your inbox to reset password" });
        }

        public async Task<ResponseDto> ResetPassword(ResetPasswordDto data)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == data.UserId);
            if (user is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User does not exist" });

            var decodedToken = HttpUtility.UrlDecode(data.Token).Replace(" ", "+");
            var result = await userManager.ResetPasswordAsync(user, decodedToken, data.NewPassword);
            if (!result.Succeeded)
            {
                logger.LogError("Could not reset password for User with ID: {userId}", data.UserId);
                return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not reset password" });
            }

            logger.LogInformation("Resetted password for User with ID: {userId}", data.UserId);
            return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Password has been set successfully" });
        }

        public async Task<ResponseDto> ResendEmailConfirmation(string email)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Email.Equals(email));
            if (user is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User does not exist" });

            if (user.EmailConfirmed) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User has already confirmed email address" });

            if (user.SentConfirmationDate.AddMinutes(1) > DateTime.UtcNow)
            {
                var secondsLeft = user.SentConfirmationDate.AddMinutes(1).Subtract(DateTime.UtcNow);
                return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { $"Cannot send confirmation email. Please try in {secondsLeft.Seconds}s" });
            }
            await SendConfirmationEmail(user);
            return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Confirmation email has been sent. Check your mailbox" });
        }

        public async Task<ResponseDto> DeleteAccount(LoginDto data)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == data.Email);
            if (user is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User does not exist" });

            var results = await signInManager.CheckPasswordSignInAsync(user, data.Password, false);
            if (!results.Succeeded) return new ResponseDto(false, StatusCodes.Status401Unauthorized, new[] { "Wrong email or password" });

            if (user.TwoFactorEnabled && string.IsNullOrEmpty(data.AuthCode)) return new ResponseDto(false, StatusCodes.Status403Forbidden, new{});
            else if (user.TwoFactorEnabled && !string.IsNullOrEmpty(data.AuthCode))
            {
                var TFAResults = await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, data.AuthCode);
                if (!TFAResults)
                {
                    return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Provided code is wrong" });
                }
            }

            db.Users.Remove(user);
            if(await db.SaveChangesAsync() > 0){
                rabbitMQSender.SendMessage(user.Id, "DeleteAccountQueue");
                logger.LogInformation("Deleted account for User with ID: {userId}", user.Id);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Account has been deleted" });
            }

            logger.LogError("Could not delete account for User with ID: {userId}", user.Id);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Account could not be deleted" });
        }

        private async Task SendConfirmationEmail(ApplicationUser user)
        {
            var email = emailProcessor.GenerateEmail(EmailTypes.Confirmation, user);
            await email.GenerateEmailData(baseUri);
            var emailDto = email.Build();
            rabbitMQSender.SendMessage(emailDto, "SendEmailQueue");

            user.SentConfirmationDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            await db.SaveChangesAsync();
        }
    }
}