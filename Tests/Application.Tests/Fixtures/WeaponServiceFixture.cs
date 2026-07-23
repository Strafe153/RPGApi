using Application.Services.Implementations;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Tests.Fixtures;

public class WeaponServiceFixture
{
    public Mock<IItemRepository<Weapon>> Repository { get; } = new();
    public Mock<ILogger<WeaponsService>> Logger { get; } = new();

    public WeaponsService CreateSut() => new(Repository.Object, Logger.Object);
}
