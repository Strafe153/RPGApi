using Application.Services.Implementations;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Tests.Fixtures;

public class SpellServiceFixture
{
    public Mock<IItemRepository<Spell>> Repository { get; } = new();
    public Mock<ILogger<SpellsService>> Logger { get; } = new();

    public SpellsService CreateSut() => new(Repository.Object, Logger.Object);
}
