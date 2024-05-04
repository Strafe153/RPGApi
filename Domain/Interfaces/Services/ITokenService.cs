using Domain.Entities;

namespace Domain.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(Player player);
    string GenerateRefreshToken();
    void SetRefreshToken(Player player, string refreshToken);
    void ValidateRefreshToken(Player player, string refreshToken);
}
