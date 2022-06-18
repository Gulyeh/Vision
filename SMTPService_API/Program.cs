using Serilog;
using SMTPService_API;
using SMTPService_API.Extensions;
using SMTPService_API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog((context, config) => {
    config.WriteTo.Console();
    config.WriteTo.Seq(builder.Configuration["SeqServer"], apiKey: builder.Configuration["SeqAPI"]);
    config.MinimumLevel.Information();
    config.Enrich.FromLogContext();
    config.Enrich.WithMachineName();
    config.Enrich.WithProcessId();
    config.Enrich.WithThreadId();
    config.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationService(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await DbMigration.Migrate(app);
app.UseMiddleware<ErrorHandler>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run("http://*:7273");
