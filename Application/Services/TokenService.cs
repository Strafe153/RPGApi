using Core.Entities;
using Core.Exceptions;
using Core.Interfaces.Services;
using Core.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<TokenService> _logger;

    public TokenService(
        IOptions<JwtOptions> jwtOptions,
        ILogger<TokenService> logger)
    {
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public string GenerateAccessToken(Player player)
    {
        List<Claim> claims = new()
        {
            new(ClaimTypes.Name, player.Name!),
            new(ClaimTypes.Role, player.Role.ToString()),
            new(nameof(player.Id), player.Id.ToString())
        };

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256Signature);

        JwtSecurityToken token = new(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            notBefore: DateTime.UtcNow,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public void SetRefreshToken(Player player, string refreshToken)
    {
        player.RefreshToken = refreshToken;
        player.RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(14);
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
