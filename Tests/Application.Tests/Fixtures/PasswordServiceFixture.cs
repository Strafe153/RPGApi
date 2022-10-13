using Application.Services;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
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
    }

    public PasswordService PasswordService { get; }
    public ILogger<PasswordService> Logger { get; }

    public string? StringPlaceholder { get; }
    public byte[] Bytes { get; }
    public byte[] PasswordHash { get; }

    private byte[] GetPasswordHash()
    {
        using var hmac = new HMACSHA256(Bytes);
        var passwordAsByteArray = Encoding.UTF8.GetBytes(StringPlaceholder!);

        return hmac.ComputeHash(passwordAsByteArray);
    }
}
