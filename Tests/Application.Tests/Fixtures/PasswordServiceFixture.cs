using Application.Services;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Core.Entities;
using Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Application.Tests.Fixtures;

public class PasswordServiceFixture
{
    public PasswordServiceFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        Configuration = fixture.Freeze<IConfiguration>();
        Logger = fixture.Freeze<ILogger<PasswordService>>();
        ConfigurationSection = fixture.Freeze<IConfigurationSection>();

        PasswordService = new(
            Configuration,
            Logger);

        StringPlaceholder = "SymmetricSecurityKey";
        Bytes = new byte[0];
        PasswordHash = GetPasswordHash();
        Player = GetPlayer();
    }

    public PasswordService PasswordService { get; }
    public IConfiguration Configuration { get; }
    public ILogger<PasswordService> Logger { get; }
    public IConfigurationSection ConfigurationSection { get; }

    public string? StringPlaceholder { get; }
    public byte[] Bytes { get; }
    public byte[] PasswordHash { get; }
    public Player Player { get; }

    private byte[] GetPasswordHash()
    {
        using (HMACSHA512 hmac = new(Bytes))
        {
            var passwordAsByteArray = Encoding.UTF8.GetBytes(StringPlaceholder!);
            return hmac.ComputeHash(passwordAsByteArray);
        }
    }

    private Player GetPlayer()
    {
        return new Player()
        {
            Id = 1,
            Name = StringPlaceholder,
            Role = PlayerRole.Player,
            PasswordHash = Bytes,
            PasswordSalt = Bytes
        };
    }
}
