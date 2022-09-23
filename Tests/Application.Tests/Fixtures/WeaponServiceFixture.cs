using Application.Services;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.Extensions.Logging;

namespace Application.Tests.Fixtures;

public class WeaponServiceFixture
{
    public WeaponServiceFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        WeaponRepository = fixture.Freeze<IRepository<Weapon>>();
        Logger = fixture.Freeze<ILogger<WeaponService>>();

        WeaponService = new WeaponService(
            WeaponRepository,
            Logger);

        Id = 1;
        Name = "StringPlaceholder";
        Weapon = GetWeapon(Id);
        Character = GetCharacter();
        PaginatedList = GetPaginatedList();
    }

    public IItemService<Weapon> WeaponService { get; }
    public IRepository<Weapon> WeaponRepository { get; }
    public ILogger<WeaponService> Logger { get; set; }

    public int Id { get; }
    public string? Name { get; }
    public Weapon Weapon { get; }
    public Character Character { get; }
    public PaginatedList<Weapon> PaginatedList { get; }

    private Character GetCharacter()
    {
        return new Character()
        {
            Id = Id,
            Name = Name,
            Race = CharacterRace.Human,
            Health = 100,
            CharacterWeapons = GetCharacterWeapons()
        };
    }

    private CharacterWeapon GetCharacterWeapon(int characterId, int weaponId)
    {
        return new CharacterWeapon()
        {
            CharacterId = characterId,
            Character = Character,
            WeaponId = weaponId,
            Weapon = GetWeapon(weaponId)
        };
    }

    private ICollection<CharacterWeapon> GetCharacterWeapons()
    {
        return new List<CharacterWeapon>()
        {
            GetCharacterWeapon(Id, 2),
            GetCharacterWeapon(Id, 3)
        };
    }

    private Weapon GetWeapon(int id)
    {
        return new Weapon()
        {
            Id = id,
            Name = Name,
            Damage = 5,
            Type = WeaponType.Sword
        };
    }

    private List<Weapon> GetWeapons()
    {
        return new List<Weapon>()
        {
            Weapon,
            Weapon
        };
    }

    private PaginatedList<Weapon> GetPaginatedList()
    {
        return new PaginatedList<Weapon>(GetWeapons(), 6, 1, 5);
    }
}
