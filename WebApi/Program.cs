using WebApi.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLoggers();

builder.Services.ConfigureHealthChecks(builder.Configuration);
builder.Services.ConfigureRateLimiting(builder.Configuration);

builder.Services.AddCustomValidators();
builder.Services.AddRepositories();
builder.Services.AddCustomServices();
builder.Services.AddMappers();

builder.Services.ConfigureApiVersioning();
builder.Services.ConfigureControllers();
builder.Services.ConfigureFluentValidation();

builder.Services.ConfigureDatabase();
builder.Services.ConfigureRedis(builder.Configuration);

builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.ConfigureSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHealthChecks();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.UseCustomMiddleware();

app.MapControllers();

app.Run();
