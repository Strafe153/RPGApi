using System.Security.Claims;

namespace Security.Interfaces;

public interface ITokenGenerator
{
	string GenerateAccessToken(IEnumerable<Claim> claims);
	string GenerateRefreshToken();
}
