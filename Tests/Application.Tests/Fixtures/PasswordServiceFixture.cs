using Application.Services;
using AutoFixture;
using AutoFixture.AutoMoq;
using Core.Entities;
using Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Cryptography;
using System.Text;

namespace Application.Tests.Fixtures
{
    public class PasswordServiceFixture
    {
        public PasswordServiceFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            MockConfiguration = fixture.Freeze<Mock<IConfiguration>>();
            MockLogger = fixture.Freeze<Mock<ILogger<PasswordService>>>();
            MockConfigurationSection = fixture.Freeze<Mock<IConfigurationSection>>();

            MockPasswordService = new(
                MockConfiguration.Object,
                MockLogger.Object);

            StringPlaceholder = "SymmetricSecurityKey";
            Bytes = new byte[0];
            PasswordHash = GetPasswordHash();
            Player = GetPlayer();
        }

        public PasswordService MockPasswordService { get; }
        public Mock<IConfiguration> MockConfiguration { get; }
        public Mock<ILogger<PasswordService>> MockLogger { get; }
        public Mock<IConfigurationSection> MockConfigurationSection { get; }

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
}
