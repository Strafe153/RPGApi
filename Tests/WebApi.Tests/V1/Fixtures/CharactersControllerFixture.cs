using Application.Services.Abstractions;
using Moq;
using WebApi.Controllers.V1;

namespace WebApi.Tests.V1.Fixtures;

public class CharactersControllerFixture
{
    public Mock<ICharactersService> CharactersService { get; } = new();

    public CharactersController CreateSut() => new(CharactersService.Object);
}
