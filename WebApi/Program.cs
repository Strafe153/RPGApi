using Application.Interfaces;
using DataAccess.Database;
using Infrastructure;
using Security.Implementations;
using Security.Interfaces;
using WebApi.Configurations;
using WebApi.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLoggers();

builder.Services.ConfigureHealthChecks(builder.Configuration);
builder.Services.ConfigureRateLimiting(builder.Configuration);

builder.Services.AddRepositories();
builder.Services.AddApplicationService();

builder.Services.AddSingleton<IAccessChecker, AccessChecker>();
builder.Services.AddSingleton<ICacheManager, CacheManager>();
builder.Services.AddSingleton<ITokenGenerator, TokenGenerator>();

builder.Services.ConfigureApiVersioning();
builder.Services.ConfigureControllers();

builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureRedis(builder.Configuration);

builder.Services.ConfigureAuthentication(builder.Configuration);

builder.Services.ConfigureSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.ConfigureSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHealthChecks();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

DatabaseInitializer.Initialize(
    app.Configuration.GetConnectionString(ConnectionConstants.DatabaseConnection)!,
    app.Services);

app.Run();
