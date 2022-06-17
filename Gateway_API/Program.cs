using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Services.AddCors();
builder.Services.AddOcelot();
var app = builder.Build();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
await app.UseOcelot();
app.Run("http://*:7271");
