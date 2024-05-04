using Domain.Dtos;
using Domain.Dtos.CharacterDtos;
using Domain.Dtos.MountDtos;
using Domain.Dtos.PlayerDtos;
using Domain.Dtos.SpellDtos;
using Domain.Dtos.WeaponDtos;
using Domain.Entities;
using Domain.Shared;
using WebApi.Mappers.CharacterMappers;
using WebApi.Mappers.Interfaces;
using WebApi.Mappers.MountMappers;
using WebApi.Mappers.PlayerMappers;
using WebApi.Mappers.SpellMappers;
using WebApi.Mappers.WeaponMappers;

namespace WebApi.Configurations;

public static class MappersConfiguration
{
    public static void AddMappers(this IServiceCollection services)
    {
        services.AddScoped<IMapper<PaginatedList<Player>, PageDto<PlayerReadDto>>, PlayerPaginatedMapper>();
        services.AddScoped<IMapper<Player, PlayerReadDto>, PlayerReadMapper>();

        services.AddScoped<IMapper<PaginatedList<Character>, PageDto<CharacterReadDto>>, CharacterPaginatedMapper>();
        services.AddScoped<IMapper<Character, CharacterReadDto>, CharacterReadMapper>();
        services.AddScoped<IMapper<CharacterCreateDto, Character>, CharacterCreateMapper>();
        services.AddScoped<IUpdateMapper<CharacterBaseDto, Character>, CharacterUpdateMapper>();

        services.AddScoped<IMapper<PaginatedList<Weapon>, PageDto<WeaponReadDto>>, WeaponPaginatedMapper>();
        services.AddScoped<IMapper<Weapon, WeaponReadDto>, WeaponReadMapper>();
        services.AddScoped<IMapper<WeaponBaseDto, Weapon>, WeaponCreateMapper>();
        services.AddScoped<IUpdateMapper<WeaponBaseDto, Weapon>, WeaponUpdateMapper>();

        services.AddScoped<IMapper<PaginatedList<Spell>, PageDto<SpellReadDto>>, SpellPaginatedMapper>();
        services.AddScoped<IMapper<Spell, SpellReadDto>, SpellReadMapper>();
        services.AddScoped<IMapper<SpellBaseDto, Spell>, SpellCreateMapper>();
        services.AddScoped<IUpdateMapper<SpellBaseDto, Spell>, SpellUpdateMapper>();

        services.AddScoped<IMapper<PaginatedList<Mount>, PageDto<MountReadDto>>, MountPaginatedMapper>();
        services.AddScoped<IMapper<Mount, MountReadDto>, MountReadMapper>();
        services.AddScoped<IMapper<MountBaseDto, Mount>, MountCreateMapper>();
        services.AddScoped<IUpdateMapper<MountBaseDto, Mount>, MountUpdateMapper>();
    }
}
