using Application.Dtos;
using Application.Dtos.MountDtos;
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

public class MountsControllerFixture
{
	public MountsControllerFixture()
	{
		var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

		Id = Random.Shared.Next();
		MountsCount = Random.Shared.Next(1, 20);

		var mountReadDtoFaker = new Faker<MountReadDto>()
			.CustomInstantiator(f => new(
				Id,
				f.Name.FirstName(),
				f.PickRandom<MountType>(),
				f.Random.Int(1, 100),
				new List<Character>()));

		var mountCreateDtoFaker = new Faker<MountCreateDto>()
			.CustomInstantiator(f => new(
				f.Name.FirstName(),
				f.PickRandom<MountType>(),
				f.Random.Int(1, 100)));

		var mountUpdateDtoFaker = new Faker<MountUpdateDto>()
			.CustomInstantiator(f => new(
				f.Name.FirstName(),
				f.PickRandom<MountType>(),
				f.Random.Int(1, 100)));

		var pageParametersFaker = new Faker<PageParameters>()
			.RuleFor(p => p.PageNumber, f => f.Random.Int(1, 100))
			.RuleFor(p => p.PageSize, f => f.Random.Int(1, 100));

		var pageDtoFaker = new Faker<PageDto<MountReadDto>>()
			.CustomInstantiator(f => new(
				1,
				f.Random.Int(1, 2),
				MountsCount,
				MountsCount,
				false,
				false,
				mountReadDtoFaker.Generate(MountsCount)));

		MountsService = fixture.Freeze<IMountsService>();

		MountsController = new(MountsService);

		MountReadDto = mountReadDtoFaker.Generate();
		MountCreateDto = mountCreateDtoFaker.Generate();
		MountUpdateDto = mountUpdateDtoFaker.Generate();
		PageParameters = pageParametersFaker.Generate();
		PageDto = pageDtoFaker.Generate();
		PatchDocument = new JsonPatchDocument<MountUpdateDto>();
	}

	public MountsController MountsController { get; }
	public IMountsService MountsService { get; }

	public int Id { get; }
	public int MountsCount { get; }
	public MountReadDto MountReadDto { get; }
	public MountCreateDto MountCreateDto { get; }
	public MountUpdateDto MountUpdateDto { get; }
	public PageParameters PageParameters { get; }
	public PageDto<MountReadDto> PageDto { get; }
	public JsonPatchDocument<MountUpdateDto> PatchDocument { get; }
	public CancellationToken CancellationToken { get; }
}
