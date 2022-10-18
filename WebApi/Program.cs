using WebApi.Configurations;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLoggers();

builder.Services.AddCustomValidators();
builder.Services.AddRepositories();
builder.Services.AddCustomServices();
builder.Services.AddMappers();

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

// AddAsync custom middleware
app.AddApplicationMiddleware();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
