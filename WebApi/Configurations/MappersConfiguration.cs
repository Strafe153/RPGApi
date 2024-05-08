using Application.Mappers.Abstractions;
using Application.Mappers.Implementations;
using Domain.Dtos.CharacterDtos;
using Domain.Dtos.MountDtos;
using Domain.Dtos.PlayerDtos;
using Domain.Dtos.SpellDtos;
using Domain.Dtos.WeaponDtos;
using Domain.Entities;

namespace WebApi.Configurations;

public static class MappersConfiguration
{
	public static void AddMappers(this IServiceCollection services) =>
		services
			.AddSingleton<IMapper<Player, PlayerReadDto, PlayerAuthorizeDto, PlayerUpdateDto>, PlayerMapper>()
			.AddSingleton<IMapper<Character, CharacterReadDto, CharacterCreateDto, CharacterUpdateDto>, CharacterMapper>()
			.AddSingleton<IMapper<Weapon, WeaponReadDto, WeaponCreateDto, WeaponUpdateDto>, WeaponMapper>()
			.AddSingleton<IMapper<Spell, SpellReadDto, SpellCreateDto, SpellUpdateDto>, SpellMapper>()
			.AddSingleton<IMapper<Mount, MountReadDto, MountCreateDto, MountUpdateDto>, MountMapper>();
}
