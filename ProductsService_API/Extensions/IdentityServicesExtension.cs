using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ProductsService_API.Extensions
{
    public static class IdentityServicesExtension
    {
         public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config){
             services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts => {
                        opts.RequireHttpsMetadata = true;
                        opts.SaveToken = true;
                        opts.TokenValidationParameters = new TokenValidationParameters{
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("PrivateKey"))),
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                    });
             return services;
         }
    }
}