using Application.Services.Implementations;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Tests.Fixtures;

public class MountServiceFixture
{
    public Mock<IItemRepository<Mount>> Repository { get; } = new();
    public Mock<ILogger<MountsService>> Logger { get; } = new();

    public MountsService CreateSut() => new(Repository.Object, Logger.Object);
}
