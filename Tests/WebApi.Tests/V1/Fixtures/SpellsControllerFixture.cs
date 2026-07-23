using Application.Services.Abstractions;
using Moq;
using WebApi.Controllers.V1;

namespace WebApi.Tests.V1.Fixtures;

public class SpellsControllerFixture
{
    public Mock<ISpellsService> SpellsService { get; } = new();

    public SpellsController CreateSut() => new(SpellsService.Object);
}
