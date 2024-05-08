using Application.Mappers.Implementations;
using Application.Services.Abstractions;
using Application.Services.Implementations;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Domain.Dtos.CharacterDtos;
using Domain.Entities;
using Domain.Enums;
using Domain.Helpers;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace Application.Tests.Fixtures;

public class CharacterServiceFixture
{
	public CharacterServiceFixture()
	{
		var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

		CharacterId = Random.Shared.Next();
		WeaponId = Random.Shared.Next();
		SpellId = Random.Shared.Next();
		CharactersCount = Random.Shared.Next(1, 20);
		PageParameters = new()
		{
			PageNumber = Random.Shared.Next(1, 25),
			PageSize = Random.Shared.Next(1, 100)
		};

		var characterFaker = new Faker<Character>();

		var weaponFaker = new Faker<Weapon>()
			.RuleFor(w => w.Id, WeaponId)
			.RuleFor(w => w.Name, f => f.Commerce.ProductName())
			.RuleFor(w => w.Damage, f => f.Random.Int(1, 100))
			.RuleFor(w => w.Type, f => f.PickRandom<WeaponType>());

		var spellFaker = new Faker<Spell>()
			.RuleFor(s => s.Id, SpellId)
			.RuleFor(s => s.Name, f => f.Commerce.ProductName())
			.RuleFor(s => s.Damage, f => f.Random.Int(1, 100))
			.RuleFor(s => s.Type, f => f.PickRandom<SpellType>());

		var mountFaker = new Faker<Mount>()
			.RuleFor(s => s.Id, f => f.Random.Int())
			.RuleFor(s => s.Name, f => f.Commerce.ProductName())
			.RuleFor(s => s.Speed, f => f.Random.Int(1, 100))
			.RuleFor(s => s.Type, f => f.PickRandom<MountType>());

		var characterWeaponFaker = new Faker<CharacterWeapon>()
			.RuleFor(cw => cw.CharacterId, CharacterId)
			.RuleFor(cw => cw.WeaponId, WeaponId)
			.RuleFor(cw => cw.Character, characterFaker)
			.RuleFor(cw => cw.Weapon, weaponFaker);

		var characterSpellFaker = new Faker<CharacterSpell>()
			.RuleFor(cw => cw.CharacterId, CharacterId)
			.RuleFor(cw => cw.SpellId, SpellId)
			.RuleFor(cw => cw.Character, characterFaker)
			.RuleFor(cw => cw.Spell, spellFaker);

		characterFaker
			.RuleFor(c => c.Id, CharacterId)
			.RuleFor(c => c.Name, f => f.Internet.UserName())
			.RuleFor(c => c.Health, f => f.Random.Int(1, 100))
			.RuleFor(c => c.Race, f => f.PickRandom<CharacterRace>())
			.RuleFor(c => c.CharacterWeapons, new[] { characterWeaponFaker.Generate() })
			.RuleFor(c => c.CharacterSpells, new[] { characterSpellFaker.Generate() });

		var characterCreateDtoFaker = new Faker<CharacterCreateDto>()
			.CustomInstantiator(f => new(
				f.Internet.UserAgent(),
				f.Random.Int(),
				f.PickRandom<CharacterRace>()));

		var characterUpdateDtoFaker = new Faker<CharacterUpdateDto>()
			.CustomInstantiator(f => new(
				f.Internet.UserAgent(),
				f.PickRandom<CharacterRace>()));

		var pagedListFaker = new Faker<PagedList<Character>>()
			.CustomInstantiator(f => new(
				characterFaker.Generate(CharactersCount),
				CharactersCount,
				f.Random.Int(1, 2),
				f.Random.Int(1, 2)));

		CharactersRepository = fixture.Freeze<IRepository<Character>>();
		PlayersRepository = fixture.Freeze<IPlayersRepository>();
		WeaponsRepository = fixture.Freeze<IItemRepository<Weapon>>();
		SpellsRepository = fixture.Freeze<IItemRepository<Spell>>();
		MountsRepository = fixture.Freeze<IItemRepository<Mount>>();
		AccessHelper = fixture.Freeze<IAccessHelper>();
		Logger = fixture.Freeze<ILogger<CharactersService>>();

		CharactersService = new CharactersService(
			CharactersRepository,
			PlayersRepository,
			WeaponsRepository,
			SpellsRepository,
			MountsRepository,
			AccessHelper,
			new CharacterMapper(),
			Logger);

		Weapon = weaponFaker.Generate();
		Spell = spellFaker.Generate();
		Mount = mountFaker.Generate();
		Character = characterFaker.Generate();
		CharacterCreateDto = characterCreateDtoFaker.Generate();
		CharacterUpdateDto = characterUpdateDtoFaker.Generate();
		PagedList = pagedListFaker.Generate();
		PatchDocument = new();
	}

	private int CharactersCount { get; }

	public ICharactersService CharactersService { get; }
	public IRepository<Character> CharactersRepository { get; }
	public IPlayersRepository PlayersRepository { get; }
	public IItemRepository<Weapon> WeaponsRepository { get; }
	public IItemRepository<Spell> SpellsRepository { get; }
	public IItemRepository<Mount> MountsRepository { get; }
	public IAccessHelper AccessHelper { get; }
	public ILogger<CharactersService> Logger { get; }
	private int WeaponId { get; }
	private int SpellId { get; }

	public int CharacterId { get; }
	public PageParameters PageParameters { get; }
	public Weapon Weapon { get; }
	public Spell Spell { get; }
	public Mount Mount { get; }
	public Character Character { get; }
	public CharacterCreateDto CharacterCreateDto { get; }
	public CharacterUpdateDto CharacterUpdateDto { get; }
	public PagedList<Character> PagedList { get; }
	public JsonPatchDocument<CharacterUpdateDto> PatchDocument { get; }
	public CancellationToken CancellationToken { get; }
}
