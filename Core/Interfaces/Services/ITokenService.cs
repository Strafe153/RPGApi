using Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(Player player);
    string GenerateRefreshToken();
    void SetRefreshToken(string refreshToken, Player player, HttpResponse response);
    void ValidateRefreshToken(Player player, string refreshToken);
}
