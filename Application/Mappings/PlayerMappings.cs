using Application.Dtos;
using Application.Dtos.PlayerDtos;
using Application.Shared;
using Domain.Entities;
using Domain.Enums;
using Domain.Shared;

namespace Application.Mappings;

public static class PlayerMappings
{
    public static PageDto<PlayerReadDto> ToPageDto(
        this GetAllResult<Player> result,
        PageParameters pageParameters)
    {
        var readDtos = result.Items.Select(ToReadDto);

        PageDto<PlayerReadDto> pageDto = new()
        {
            PageSize = pageParameters.PageSize,
            CurrentPage = pageParameters.PageNumber,
            TotalItems = result.Count,
            Items = readDtos,
        };

        return pageDto;
    }

    public static PlayerReadDto ToReadDto(this Player entity) => new(
        entity.Id,
        entity.Name,
        entity.Role,
        entity.Characters);

    public static Player ToPlayer(this PlayerAuthorizeDto dto) => new()
    {
        Name = dto.Name,
        Role = PlayerRole.Player,
    };

    public static void Update(this PlayerUpdateDto dto, Player entity) => entity.Name = dto.Name;
}
