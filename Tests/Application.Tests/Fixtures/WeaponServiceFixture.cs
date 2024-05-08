using Application.Mappers.Implementations;
using Application.Services.Abstractions;
using Application.Services.Implementations;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Domain.Dtos.WeaponDtos;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace Application.Tests.Fixtures;

public class WeaponServiceFixture
{
	public WeaponServiceFixture()
	{
		var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

		Id = Random.Shared.Next();
		WeaponsCount = Random.Shared.Next(1, 20);
		PageParameters = new()
		{
			PageNumber = Random.Shared.Next(1, 25),
			PageSize = Random.Shared.Next(1, 100)
		};

		var weaponFaker = new Faker<Weapon>()
			.RuleFor(w => w.Id, Id)
			.RuleFor(w => w.Name, f => f.Commerce.ProductName())
			.RuleFor(w => w.Damage, f => f.Random.Int(1, 100))
			.RuleFor(w => w.Type, f => f.PickRandom<WeaponType>());

		var weaponCreateDtoFaker = new Faker<WeaponCreateDto>()
			.CustomInstantiator(f => new(
				f.Commerce.ProductName(),
				f.PickRandom<WeaponType>(),
				f.Random.Int(1, 100)));

		var weaponUpdateDtoFaker = new Faker<WeaponUpdateDto>()
			.CustomInstantiator(f => new(
				f.Commerce.ProductName(),
				f.PickRandom<WeaponType>(),
				f.Random.Int(1, 100)));

		var pagedListFaker = new Faker<PagedList<Weapon>>()
			.CustomInstantiator(f => new(
				weaponFaker.Generate(WeaponsCount),
				WeaponsCount,
				f.Random.Int(1, 2),
				f.Random.Int(1, 2)));

		WeaponsRepository = fixture.Freeze<IItemRepository<Weapon>>();
		Logger = fixture.Freeze<ILogger<WeaponsService>>();

		WeaponsService = new WeaponsService(
			WeaponsRepository,
			new WeaponMapper(),
			Logger);

		Weapon = weaponFaker.Generate();
		WeaponCreateDto = weaponCreateDtoFaker.Generate();
		WeaponUpdateDto = weaponUpdateDtoFaker.Generate();
		Weapons = weaponFaker.Generate(WeaponsCount);
		PagedList = pagedListFaker.Generate();
		PatchDocument = new();
	}

	private int WeaponsCount { get; }

	public IWeaponsService WeaponsService { get; }
	public IItemRepository<Weapon> WeaponsRepository { get; }
	public ILogger<WeaponsService> Logger { get; set; }

	public int Id { get; }
	public PageParameters PageParameters { get; }
	public Weapon Weapon { get; }
	public WeaponCreateDto WeaponCreateDto { get; }
	public WeaponUpdateDto WeaponUpdateDto { get; }
	public List<Weapon> Weapons { get; }
	public PagedList<Weapon> PagedList { get; }
	public JsonPatchDocument<WeaponUpdateDto> PatchDocument { get; }
	public CancellationToken CancellationToken { get; }
}
