using Application.Services.Abstractions;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Domain.Dtos;
using Domain.Dtos.WeaponDtos;
using Domain.Entities;
using Domain.Enums;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;
using WebApi.Controllers.V1;

namespace WebApi.Tests.V1.Fixtures;

public class WeaponsControllerFixture
{
	public WeaponsControllerFixture()
	{
		var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

		Id = Random.Shared.Next();
		WeaponsCount = Random.Shared.Next(1, 20);

		var weaponReadDtoFaker = new Faker<WeaponReadDto>()
			.CustomInstantiator(f => new(
				Id,
				f.Commerce.ProductName(),
				f.PickRandom<WeaponType>(),
				f.Random.Int(1, 100),
				new List<Character>()));

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

		var pageParametersFaker = new Faker<PageParameters>()
			.RuleFor(p => p.PageNumber, f => f.Random.Int(1, 100))
			.RuleFor(p => p.PageSize, f => f.Random.Int(1, 100));

		var pageDtoFaker = new Faker<PageDto<WeaponReadDto>>()
			.CustomInstantiator(f => new(
				1,
				f.Random.Int(1, 2),
				WeaponsCount,
				WeaponsCount,
				false,
				false,
				weaponReadDtoFaker.Generate(WeaponsCount)));

		WeaponsService = fixture.Freeze<IWeaponsService>();

		WeaponsController = new(WeaponsService);

		WeaponReadDto = weaponReadDtoFaker.Generate();
		WeaponCreateDto = weaponCreateDtoFaker.Generate();
		WeaponUpdateDto = weaponUpdateDtoFaker.Generate();
		PageParameters = pageParametersFaker.Generate();
		PageDto = pageDtoFaker.Generate();
		PatchDocument = new JsonPatchDocument<WeaponUpdateDto>();
	}

	public WeaponsController WeaponsController { get; }
	public IWeaponsService WeaponsService { get; }

	public int Id { get; }
	public int WeaponsCount { get; }
	public WeaponReadDto WeaponReadDto { get; }
	public WeaponCreateDto WeaponCreateDto { get; }
	public WeaponUpdateDto WeaponUpdateDto { get; }
	public PageParameters PageParameters { get; }
	public PageDto<WeaponReadDto> PageDto { get; }
	public JsonPatchDocument<WeaponUpdateDto> PatchDocument { get; }
	public CancellationToken CancellationToken { get; }
}
