using Application.Dtos;
using Application.Dtos.SpellDtos;
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

public class SpellsControllerFixture
{
	public SpellsControllerFixture()
	{
		var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

		Id = Random.Shared.Next();
		SpellsCount = Random.Shared.Next(1, 20);

		var spellReadDtoFaker = new Faker<SpellReadDto>()
			.CustomInstantiator(f => new(
				Id,
				f.Commerce.ProductName(),
				f.PickRandom<SpellType>(),
				f.Random.Int(1, 100),
				new List<Character>()));

		var spellCreateDtoFaker = new Faker<SpellCreateDto>()
			.CustomInstantiator(f => new(
				f.Commerce.ProductName(),
				f.PickRandom<SpellType>(),
				f.Random.Int(1, 100)));

		var spellUpdateDtoFaker = new Faker<SpellUpdateDto>()
			.CustomInstantiator(f => new(
				f.Commerce.ProductName(),
				f.PickRandom<SpellType>(),
				f.Random.Int(1, 100)));

		var pageParametersFaker = new Faker<PageParameters>()
			.RuleFor(p => p.PageNumber, f => f.Random.Int(1, 100))
			.RuleFor(p => p.PageSize, f => f.Random.Int(1, 100));

		var pageDtoFaker = new Faker<PageDto<SpellReadDto>>()
			.CustomInstantiator(f => new(
				1,
				f.Random.Int(1, 2),
				SpellsCount,
				SpellsCount,
				false,
				false,
				spellReadDtoFaker.Generate(SpellsCount)));

		SpellsService = fixture.Freeze<ISpellsService>();

		SpellsController = new(SpellsService);

		SpellReadDto = spellReadDtoFaker.Generate();
		SpellCreateDto = spellCreateDtoFaker.Generate();
		SpellUpdateDto = spellUpdateDtoFaker.Generate();
		PageParameters = pageParametersFaker.Generate();
		PageDto = pageDtoFaker.Generate();
		PatchDocument = new JsonPatchDocument<SpellUpdateDto>();
	}

	public SpellsController SpellsController { get; }
	public ISpellsService SpellsService { get; }

	public int Id { get; }
	public int SpellsCount { get; }
	public SpellReadDto SpellReadDto { get; }
	public SpellCreateDto SpellCreateDto { get; }
	public SpellUpdateDto SpellUpdateDto { get; }
	public PageParameters PageParameters { get; }
	public PageDto<SpellReadDto> PageDto { get; }
	public JsonPatchDocument<SpellUpdateDto> PatchDocument { get; }
	public CancellationToken CancellationToken { get; }
}
