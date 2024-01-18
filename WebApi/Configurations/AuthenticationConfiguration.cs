using Core.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApi.Configurations;

public static class AuthenticationConfiguration
{
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        var jwtOptinos = jwtSection.Get<JwtOptions>()!;

        services.Configure<JwtOptions>(jwtSection);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateActor = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptinos.Issuer,
                    ValidAudience = jwtOptinos.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptinos.Secret))
                });
    }
}
