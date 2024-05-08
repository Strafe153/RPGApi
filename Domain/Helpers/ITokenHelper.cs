using Domain.Entities;

namespace Domain.Helpers;

public interface ITokenHelper
{
	string GenerateAccessToken(Player player);
	string GenerateRefreshToken();
	void SetRefreshToken(Player player, string refreshToken);
	void ValidateRefreshToken(Player player, string refreshToken);
}
