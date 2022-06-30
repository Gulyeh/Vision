using CodesService_API.Statics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CodesService_API.Extensions
{
    public static class IdentityServicesExtension
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                   .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
                   {
                       opts.RequireHttpsMetadata = true;
                       opts.SaveToken = true;
                       opts.TokenValidationParameters = new TokenValidationParameters
                       {
                           ValidateIssuerSigningKey = true,
                           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("PrivateKey"))),
                           ValidateIssuer = false,
                           ValidateAudience = false
                       };
                   });

            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("HasOwnerRole", builder => builder.RequireRole(StaticData.OwnerRole));
                opts.AddPolicy("HasAdminRole", builder => builder.RequireRole(StaticData.AdminRole, StaticData.OwnerRole));
                opts.AddPolicy("HasModeratorRole", builder => builder.RequireRole(StaticData.OwnerRole, StaticData.AdminRole, StaticData.ModeratorRole));
            });
            return services;
        }
    }
}