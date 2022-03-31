using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts => {
                    opts.RequireHttpsMetadata = true;
                    opts.SaveToken = true;
                    opts.TokenValidationParameters = new TokenValidationParameters{
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                }); 
builder.Services.AddOcelot();                           
var app = builder.Build();
await app.UseOcelot();

app.Run();
