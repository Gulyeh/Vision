using CodesService_API;
using CodesService_API.Extensions;
using CodesService_API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Host.UseSerilog((context, config) =>
{
    config.WriteTo.Console();
    config.WriteTo.Seq(builder.Configuration["SeqServer"], apiKey: builder.Configuration["SeqAPI"]);
    config.MinimumLevel.Information();
    config.Enrich.FromLogContext();
    config.Enrich.WithMachineName();
    config.Enrich.WithProcessId();
    config.Enrich.WithThreadId();
    config.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning);
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Codes Service", Version = "v1" });
    c.EnableAnnotations();
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "Enter your JWT token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            new List<string>()
        }
    });
});
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
await DbMigration.Migrate(app);
app.UseMiddleware<ErrorHandler>();
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run("http://*:7128");
