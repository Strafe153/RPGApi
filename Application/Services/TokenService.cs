using Core.Entities;
using Core.Exceptions;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenService> _logger;

    public TokenService(
        IConfiguration configuration,
        ILogger<TokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateAccessToken(Player player)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, player.Name!),
            new Claim(ClaimTypes.Role, player.Role.ToString()),
            new Claim("id", player.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings:Secret").Value));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            issuer: _configuration.GetSection("JwtSettings:Issuer").Value,
            audience: _configuration.GetSection("JwtSettings:Audience").Value,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            notBefore: DateTime.UtcNow,
            signingCredentials: credentials);

        string jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    public string GenerateRefreshToken()
    {
        byte[] bytesForToken = RandomNumberGenerator.GetBytes(64);
        string refreshToken = Convert.ToBase64String(bytesForToken);

        return refreshToken;
    }

    public void SetRefreshToken(string refreshToken, Player player, HttpResponse response)
    {
        DateTime expiryDate = DateTime.UtcNow.AddDays(7);
        var cookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            Expires = expiryDate
        };

        player.RefreshToken = refreshToken;
        player.RefreshTokenExpiryDate = expiryDate;

        response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

    public void ValidateRefreshToken(Player player, string refreshToken)
    {
        if (player.RefreshToken != refreshToken)
        {
            _logger.LogWarning("Player '{Name}' failed to validate a refresh token due to providing an incorrect one", player.Name);
            throw new InvalidTokenException("Token is not valid");
        }

        if (player.RefreshTokenExpiryDate < DateTime.UtcNow)
        {
            _logger.LogWarning("Player '{Name}' failed to validate a refresh token due to token expiration", player.Name);
            throw new InvalidTokenException("Token has expired");
        }

        _logger.LogInformation("Player '{Name}' successfully validated a refresh token", player.Name);
    }
}
