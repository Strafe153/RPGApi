﻿using Application.Services;
using AutoFixture;
using AutoFixture.AutoMoq;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Security.Principal;

namespace Application.Tests.Fixtures
{
    public class PlayerServiceFixture
    {
        public PlayerServiceFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            MockPlayerRepository = fixture.Freeze<Mock<IPlayerRepository>>();
            MockLogger = fixture.Freeze<Mock<ILogger<PlayerService>>>();

            MockPlayerService = new PlayerService(
                MockPlayerRepository.Object,
                MockLogger.Object);

            Id = 1;
            Name = "StringPlaceholder";
            Bytes = new byte[0];
            Race = CharacterRace.Human;
            Player = GetPlayer();
            Players = GetPlayers();
            PagedList = GetPagedList();
            IIdentity = GetClaimsIdentity();
            SufficientClaims = GetSufficientClaims();
            InsufficientClaims = GetInsufficientClaims();
        }

        public IPlayerService MockPlayerService { get; }
        public Mock<IPlayerRepository> MockPlayerRepository { get; }
        public Mock<ILogger<PlayerService>> MockLogger { get; }

        public int Id { get; }
        public string? Name { get; }
        public byte[] Bytes { get; }
        public CharacterRace Race { get; }
        public Player Player { get; }
        public List<Player> Players { get; }
        public PagedList<Player> PagedList { get; }
        public IIdentity IIdentity { get; }
        public IEnumerable<Claim> SufficientClaims { get; }
        public IEnumerable<Claim> InsufficientClaims { get; }

        private Character GetCharacter()
        {
            return new Character()
            {
                Id = Id,
                Name = Name,
                Race = Race,
                Health = 100,
                PlayerId = Id
            };
        }

        private ICollection<Character> GetCharacters()
        {
            return new List<Character>()
            {
                GetCharacter(),
                GetCharacter()
            };
        }

        private Player GetPlayer()
        {
            return new Player()
            {
                Id = Id,
                Name = Name,
                Role = PlayerRole.Player,
                PasswordHash = Bytes,
                PasswordSalt = Bytes,
                Characters = GetCharacters()
            };
        }

        private List<Player> GetPlayers()
        {
            return new List<Player>()
            {
                Player,
                Player
            };
        }

        private PagedList<Player> GetPagedList()
        {
            return new PagedList<Player>(Players, 6, 1, 5);
        }

        private IIdentity GetClaimsIdentity()
        {
            return new ClaimsIdentity(Name, Name, Name);
        }

        private IEnumerable<Claim> GetInsufficientClaims()
        {
            return new List<Claim>()
            {
                new Claim(ClaimTypes.Name, Name!),
                new Claim(ClaimTypes.Role, Name!)
            };
        }

        private IEnumerable<Claim> GetSufficientClaims()
        {
            return new List<Claim>()
            {
                new Claim(ClaimTypes.Name, string.Empty),
                new Claim(ClaimTypes.Role, PlayerRole.Admin.ToString())
            };
        }
    }
}