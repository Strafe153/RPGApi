using Application.Mappers.Implementations;
using Application.Services.Abstractions;
using Application.Services.Implementations;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Domain.Dtos.MountDtos;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace Application.Tests.Fixtures;

public class MountServiceFixture
{
	public MountServiceFixture()
	{
		var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

		Id = Random.Shared.Next();
		MountsCount = Random.Shared.Next(1, 20);
		PageParameters = new()
		{
			PageNumber = Random.Shared.Next(1, 25),
			PageSize = Random.Shared.Next(1, 100)
		};

		var mountFaker = new Faker<Mount>()
			.RuleFor(m => m.Id, Id)
			.RuleFor(m => m.Name, f => f.Name.FirstName())
			.RuleFor(m => m.Speed, f => f.Random.Int(1, 100))
			.RuleFor(m => m.Type, f => f.PickRandom<MountType>());

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

		var characterFaker = new Faker<Character>()
			.RuleFor(c => c.Id, f => f.Random.Int())
			.RuleFor(c => c.Name, f => f.Internet.UserName())
			.RuleFor(c => c.Health, f => f.Random.Int(1, 100))
			.RuleFor(c => c.Race, f => f.PickRandom<CharacterRace>());

		var pagedListFaker = new Faker<PagedList<Mount>>()
			.CustomInstantiator(f => new(
				mountFaker.Generate(MountsCount),
				MountsCount,
				f.Random.Int(1, 2),
				f.Random.Int(1, 2)));

		MountsRepository = fixture.Freeze<IItemRepository<Mount>>();
		Logger = fixture.Freeze<ILogger<MountsService>>();

		MountsService = new MountsService(MountsRepository, new MountMapper(), Logger);

		Mount = mountFaker.Generate();
		MountCreateDto = mountCreateDtoFaker.Generate();
		MountUpdateDto = mountUpdateDtoFaker.Generate();
		Character = characterFaker.Generate();
		Mounts = mountFaker.Generate(MountsCount);
		PagedList = pagedListFaker.Generate();
		PatchDocument = new();
	}

	private int MountsCount { get; }

	public IMountsService MountsService { get; }
	public IItemRepository<Mount> MountsRepository { get; }
	public ILogger<MountsService> Logger { get; }

	public int Id { get; }
	public PageParameters PageParameters { get; }
	public Mount Mount { get; }
	public MountCreateDto MountCreateDto { get; }
	public MountUpdateDto MountUpdateDto { get; }
	public Character Character { get; }
	public List<Mount> Mounts { get; }
	public PagedList<Mount> PagedList { get; }
	public JsonPatchDocument<MountUpdateDto> PatchDocument { get; }
	public CancellationToken CancellationToken { get; }
}
