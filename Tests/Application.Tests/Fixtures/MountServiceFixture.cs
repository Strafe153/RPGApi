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

namespace Application.Tests.Fixtures
{
    public class MountServiceFixture
    {
        public MountServiceFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            MockMountRepository = fixture.Freeze<Mock<IRepository<Mount>>>();
            MockLogger = fixture.Freeze<Mock<ILogger<MountService>>>();

            MockMountService = new MountService(
                MockMountRepository.Object,
                MockLogger.Object);

            Id = 1;
            Name = "StringPlaceholder";
            Mount = GetMount(Id);
            Character = GetCharacter();
            Mounts = GetMounts();
            PagedList = GetPagedList();
    }

        public IItemService<Mount> MockMountService { get; }
        public Mock<IRepository<Mount>> MockMountRepository { get; }
        public Mock<ILogger<MountService>> MockLogger { get; }

        public int Id { get; }
        public string? Name { get; }
        public Mount Mount { get; }
        public Character Character { get; }
        public List<Mount> Mounts { get; }
        public PagedList<Mount> PagedList { get; }

        private CharacterMount GetCharacterMount(int characterId, int mountId)
        {
            return new CharacterMount()
            {
                CharacterId = characterId,
                Character = Character,
                MountId = mountId,
                Mount = GetMount(mountId)
            };
        }

        private ICollection<CharacterMount> GetCharacterMounts()
        {
            return new List<CharacterMount>()
            {
                GetCharacterMount(Id, 2),
                GetCharacterMount(Id, 3)
            };
        }

        private Character GetCharacter()
        {
            return new Character()
            {
                Id = Id,
                Name = Name,
                Race = CharacterRace.Human,
                Health = 100,
                CharacterMounts = GetCharacterMounts()
            };
        }

        private Mount GetMount(int id)
        {
            return new Mount()
            {
                Id = id,
                Name = Name,
                Speed = Id,
                Type = MountType.Horse
            };
        }

        private List<Mount> GetMounts()
        {
            return new List<Mount>()
            {
                Mount,
                Mount
            };
        }

        private PagedList<Mount> GetPagedList()
        {
            return new PagedList<Mount>(Mounts, 6, 1, 5);
        }
    }
}