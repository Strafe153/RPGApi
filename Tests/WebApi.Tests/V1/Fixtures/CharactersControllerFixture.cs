using Application.Dtos;
using Application.Dtos.CharactersDtos;
using Application.Services.Abstractions;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Domain.Entities;
using Domain.Enums;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;
using WebApi.Controllers.V1;

namespace WebApi.Tests.V1.Fixtures;

public class CharactersControllerFixture
{
	public CharactersControllerFixture()
	{
		var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

		Id = Random.Shared.Next();
		CharactersCount = Random.Shared.Next(1, 20);

		var weaponFaker = new Faker<Weapon>()
			.RuleFor(w => w.Id, f => f.Random.Int())
			.RuleFor(w => w.Name, f => f.Commerce.ProductName())
			.RuleFor(w => w.Damage, f => f.Random.Int(1, 100))
			.RuleFor(w => w.Type, f => f.PickRandom<WeaponType>());

		var spellFaker = new Faker<Spell>()
			.RuleFor(s => s.Id, f => f.Random.Int())
			.RuleFor(s => s.Name, f => f.Commerce.ProductName())
			.RuleFor(s => s.Damage, f => f.Random.Int(1, 100))
			.RuleFor(s => s.Type, f => f.PickRandom<SpellType>());

		var mountFaker = new Faker<Mount>()
			.RuleFor(m => m.Id, f => f.Random.Int())
			.RuleFor(m => m.Name, f => f.Name.FirstName())
			.RuleFor(m => m.Speed, f => f.Random.Int(1, 100))
			.RuleFor(m => m.Type, f => f.PickRandom<MountType>());

		var characterReadDtoFaker = new Faker<CharacterReadDto>()
			.CustomInstantiator(f => new(
				Id,
				f.Internet.UserName(),
				f.PickRandom<CharacterRace>(),
				f.Random.Int(1, 100),
				f.Random.Int(),
				new List<Weapon>(),
				new List<Spell>(),
				new List<Mount>()));

		var characterCreateDtoFaker = new Faker<CharacterCreateDto>()
			.CustomInstantiator(f => new(
				f.Internet.UserName(),
				f.Random.Int(),
				f.PickRandom<CharacterRace>()));

		var characterUpdateDtoFaker = new Faker<CharacterUpdateDto>()
			.CustomInstantiator(f => new(
				f.Internet.UserName(),
				f.PickRandom<CharacterRace>()));

		var addRemoveItemDtoFaker = new Faker<ManageItemDto>()
			.CustomInstantiator(f => new(
				Id,
				f.Random.Int(),
				f.PickRandom<ItemType>(),
				f.PickRandom<ManageItemOperation>()));

		var pageParametersFaker = new Faker<PageParameters>()
			.RuleFor(p => p.PageNumber, f => f.Random.Int(1, 100))
			.RuleFor(p => p.PageSize, f => f.Random.Int(1, 100));

		var pageDtoFaker = new Faker<PageDto<CharacterReadDto>>()
			.CustomInstantiator(f => new(
				1,
				f.Random.Int(1, 2),
				CharactersCount,
				CharactersCount,
				false,
				false,
				characterReadDtoFaker.Generate(CharactersCount)));

		CharactersService = fixture.Freeze<ICharactersService>();

		CharactersController = new(CharactersService);

		Weapon = weaponFaker.Generate();
		Spell = spellFaker.Generate();
		Mount = mountFaker.Generate();
		CharacterReadDto = characterReadDtoFaker.Generate();
		CharacterCreateDto = characterCreateDtoFaker.Generate();
		CharacterUpdateDto = characterUpdateDtoFaker.Generate();
		PatchDocument = new JsonPatchDocument<CharacterUpdateDto>();
		ItemDto = addRemoveItemDtoFaker.Generate();
		PageParameters = pageParametersFaker.Generate();
		PageDto = pageDtoFaker.Generate();
	}

	public CharactersController CharactersController { get; }
	public ICharactersService CharactersService { get; }

	public int Id { get; }
	public int CharactersCount { get; }
	public Weapon Weapon { get; }
	public Spell Spell { get; }
	public Mount Mount { get; }
	public CharacterReadDto CharacterReadDto { get; }
	public CharacterCreateDto CharacterCreateDto { get; }
	public CharacterUpdateDto CharacterUpdateDto { get; }
	public JsonPatchDocument<CharacterUpdateDto> PatchDocument { get; }
	public ManageItemDto ItemDto { get; }
	public PageParameters PageParameters { get; }
	public PageDto<CharacterReadDto> PageDto { get; }
	public CancellationToken CancellationToken { get; }
}
