using Application.Dtos;
using Application.Dtos.PlayerDtos;
using Application.Dtos.TokenDtos;
using Domain.Shared;

namespace Application.Services.Abstractions;

public interface IPlayersService
{
	Task<PageDto<PlayerReadDto>> GetAllAsync(PageParameters pageParameters, CancellationToken token);
	Task<PlayerReadDto> GetByIdAsync(int id, CancellationToken token);
	Task<PlayerReadDto> AddAsync(PlayerAuthorizeDto createDto);
	Task UpdateAsync(int id, PlayerUpdateDto updateDto, CancellationToken token);
	Task DeleteAsync(int id, CancellationToken token);
	Task<TokensReadDto> LoginAsync(PlayerAuthorizeDto authorizeDto, CancellationToken token);
	Task<TokensReadDto> RefreshTokensAsync(int id, TokensRefreshDto refreshDto, CancellationToken token);
	Task<TokensReadDto> ChangePasswordAsync(int id, PlayerChangePasswordDto passwordDto, CancellationToken token);
	Task<PlayerReadDto> ChangeRoleAsync(int id, PlayerChangeRoleDto roleDto, CancellationToken token);
}
