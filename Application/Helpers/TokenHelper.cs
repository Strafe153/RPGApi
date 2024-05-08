using Domain.Entities;
using Domain.Exceptions;
using Domain.Helpers;
using Domain.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Helpers;

public class TokenHelper : ITokenHelper
{
	private readonly JwtOptions _jwtOptions;
	private readonly ILogger<TokenHelper> _logger;

	public TokenHelper(
		IOptions<JwtOptions> jwtOptions,
		ILogger<TokenHelper> logger)
	{
		_jwtOptions = jwtOptions.Value;
		_logger = logger;
	}

	public string GenerateAccessToken(Player player)
	{
		List<Claim> claims = new()
		{
			new(ClaimTypes.Name, player.Name),
			new(ClaimTypes.Role, player.Role.ToString()),
			new(nameof(player.Id), player.Id.ToString())
		};

		SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
		SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256Signature);

		JwtSecurityToken token = new(
			issuer: _jwtOptions.Issuer,
			audience: _jwtOptions.Audience,
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessExpirationPeriod),
			notBefore: DateTime.UtcNow,
			signingCredentials: credentials);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	public string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

	public void SetRefreshToken(Player player, string refreshToken)
	{
		player.RefreshToken = refreshToken;
		player.RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(_jwtOptions.RefreshExpirationPeriod);
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
