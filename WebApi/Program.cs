using Autofac.Extensions.DependencyInjection;
using DataAccess.Database;
using Domain.Constants;
using WebApi.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAutofac();
builder.ConfigureLoggers();

builder.Services.ConfigureHealthChecks(builder.Configuration);
builder.Services.ConfigureRateLimiting(builder.Configuration);

builder.Services.ConfigureApiVersioning();
builder.Services.ConfigureControllers();
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
    app.Configuration.GetConnectionString(ConnectionStringConstants.DatabaseConnection)!,
    app.Services.GetAutofacRoot());

app.Run();