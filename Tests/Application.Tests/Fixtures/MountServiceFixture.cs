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

public class MountServiceFixture
{
    public MountServiceFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        MountRepository = fixture.Freeze<IRepository<Mount>>();
        Logger = fixture.Freeze<ILogger<MountService>>();

        MountService = new MountService(
            MountRepository,
            Logger);

        Id = 1;
        Name = "StringPlaceholder";
        Mount = GetMount(Id);
        Character = GetCharacter();
        Mounts = GetMounts();
        PaginatedList = GetPaginatedList();
}

    public IItemService<Mount> MountService { get; }
    public IRepository<Mount> MountRepository { get; }
    public ILogger<MountService> Logger { get; }

    public int Id { get; }
    public string? Name { get; }
    public Mount Mount { get; }
    public Character Character { get; }
    public List<Mount> Mounts { get; }
    public PaginatedList<Mount> PaginatedList { get; }

    private CharacterMount GetCharacterMount(int characterId, int mountId)
    {
        return new CharacterMount()
        {
            CharacterId = characterId,
            Character = Character,
            MountId = mountId,
            Mount = GetMount(mountId)
        };
    }

    private ICollection<CharacterMount> GetCharacterMounts()
    {
        return new List<CharacterMount>()
        {
            GetCharacterMount(Id, 2),
            GetCharacterMount(Id, 3)
        };
    }

    private Character GetCharacter()
    {
        return new Character()
        {
            Id = Id,
            Name = Name,
            Race = CharacterRace.Human,
            Health = 100,
            CharacterMounts = GetCharacterMounts()
        };
    }

    private Mount GetMount(int id)
    {
        return new Mount()
        {
            Id = id,
            Name = Name,
            Speed = Id,
            Type = MountType.Horse
        };
    }

    private List<Mount> GetMounts()
    {
        return new List<Mount>()
        {
            Mount,
            Mount
        };
    }

    private PaginatedList<Mount> GetPaginatedList()
    {
        return new PaginatedList<Mount>(Mounts, 6, 1, 5);
    }
}
