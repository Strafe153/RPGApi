using Application.Services.Abstractions;
using Moq;
using WebApi.Controllers.V1;

namespace WebApi.Tests.V1.Fixtures;

public class WeaponsControllerFixture
{
    public Mock<IWeaponsService> WeaponsService { get; } = new();

    public WeaponsController CreateSut() => new(WeaponsService.Object);
}
