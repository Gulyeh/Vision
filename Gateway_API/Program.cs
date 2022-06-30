using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
var builder = WebApplication.CreateBuilder(args);
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

builder.Services.AddCors();
builder.Services.AddOcelot();
var app = builder.Build();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
await app.UseOcelot();
app.Run("http://*:7271");
