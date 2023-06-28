using Application.Services;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Core.Entities;
using Core.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Tests.Fixtures;

public class PasswordServiceFixture
{
    public PasswordServiceFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        var playerFaker = new Faker<Player>()
            .RuleFor(p => p.Id, f => f.Random.Int())
            .RuleFor(p => p.Name, f => f.Internet.UserName())
            .RuleFor(p => p.Role, f => (PlayerRole)f.Random.Int(Enum.GetValues(typeof(PlayerRole)).Length))
            .RuleFor(p => p.PasswordHash, f => f.Random.Bytes(32))
            .RuleFor(p => p.PasswordSalt, f => f.Random.Bytes(32))
            .RuleFor(p => p.RefreshToken, f => f.Random.String(64))
            .RuleFor(p => p.RefreshTokenExpiryDate, f => f.Date.Future());

        Logger = fixture.Freeze<ILogger<PasswordService>>();

        PasswordService = new PasswordService(Logger);

        Password = new Faker().Internet.Password();
        Player = playerFaker.Generate();
    }

    public PasswordService PasswordService { get; }
    public ILogger<PasswordService> Logger { get; }

    public string Password { get; }
    public Player Player { get; }
}
