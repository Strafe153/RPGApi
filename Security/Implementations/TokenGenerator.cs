using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Security.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Security.Implementations;

public class TokenGenerator : ITokenGenerator
{
    private static readonly JwtSecurityTokenHandler _handler = new();

	private readonly JwtOptions _jwtOptions;

	public TokenGenerator(IOptions<JwtOptions> jwtOptions)
	{
		_jwtOptions = jwtOptions.Value;
	}

	public string GenerateAccessToken(IEnumerable<Claim> claims)
	{
		SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
		SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256Signature);

		JwtSecurityToken token = new(
			issuer: _jwtOptions.Issuer,
			audience: _jwtOptions.Audience,
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessExpirationPeriod),
			notBefore: DateTime.UtcNow,
			signingCredentials: credentials);

		return _handler.WriteToken(token);
	}

	public string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}