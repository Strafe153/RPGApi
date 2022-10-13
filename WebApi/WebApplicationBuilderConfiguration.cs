using Application.Services;
using DataAccess;
using DataAccess.Repositories;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog.Web;
using System.Text;
using WebApi.Mappers;
using WebApi.Validators;

namespace WebApi;

public static class WebApplicationBuilderConfiguration
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        // Add NLog.
        builder.Logging.ClearProviders();
        builder.Host.UseNLog();

        // Configure Database
        builder.Services.AddSingleton<RPGContext>();

        // Add custom validators, repositories, services, mappers.
        builder.Services.AddApplicationValidators()
            .AddApplicationRepositories()
            .AddApplicationServices()
            .AddApplicationMappers();

        // AddAsync, configure services for controllers.
        builder.Services.AddControllers(options =>
        {
            options.SuppressAsyncSuffixInActionNames = false;
        })
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        });

        // Add distributed Redis cache.
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
        });

        // Add JWT-token authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateActor = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration.GetSection("JwtSettings:Issuer").Value,
                    ValidAudience = builder.Configuration.GetSection("JwtSettings:Audience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtSettings:Secret").Value))
                };
            });

        // Add fluent validation
        builder.Services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();

        // Add and configure Swagger.
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "JWTToken_Auth_API",
                Version = "v1"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}
