using Application.Services;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Core.Entities;
using Core.Enums;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Application.Tests.Fixtures;

public class PasswordServiceFixture
{
    public PasswordServiceFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        Logger = fixture.Freeze<ILogger<PasswordService>>();

        PasswordService = new PasswordService(Logger);

        StringPlaceholder = "SymmetricSecurityKey";
        Bytes = Array.Empty<byte>();
        PasswordHash = GetPasswordHash();
        Player = GetPlayer();
    }

    public PasswordService PasswordService { get; }
    public ILogger<PasswordService> Logger { get; }

    public string? StringPlaceholder { get; }
    public Player Player { get; }
    public byte[] Bytes { get; }
    public byte[] PasswordHash { get; }

    private byte[] GetPasswordHash()
    {
        using var hmac = new HMACSHA256(Bytes);
        var passwordAsByteArray = Encoding.UTF8.GetBytes(StringPlaceholder!);

        return hmac.ComputeHash(passwordAsByteArray);
    }

    private Player GetPlayer()
    {
        return new Player()
        {
            Id = 1,
            Name = StringPlaceholder,
            Role = PlayerRole.Player,
            PasswordHash = Bytes,
            PasswordSalt = Bytes,
            RefreshTokenExpiryDate = DateTime.Today,
            RefreshToken = StringPlaceholder
        };
    }
}
