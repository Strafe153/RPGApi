using Application.Services.Abstractions;
using Moq;
using WebApi.Controllers.V1;

namespace WebApi.Tests.V1.Fixtures;

public class MountsControllerFixture
{
    public Mock<IMountsService> MountsService { get; } = new();

    public MountsController CreateSut() => new(MountsService.Object);
}
