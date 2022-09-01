using Application.Services;
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
    public class WeaponServiceFixture
    {
        public WeaponServiceFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            MockWeaponRepository = fixture.Freeze<Mock<IRepository<Weapon>>>();
            MockLogger = fixture.Freeze<Mock<ILogger<WeaponService>>>();

            MockWeaponService = new WeaponService(
                MockWeaponRepository.Object,
                MockLogger.Object);

            Id = 1;
            Name = "StringPlaceholder";
            Weapon = GetWeapon(Id);
            Character = GetCharacter();
            PaginatedList = GetPaginatedList();
        }

        public IItemService<Weapon> MockWeaponService { get; }
        public Mock<IRepository<Weapon>> MockWeaponRepository { get; }
        public Mock<ILogger<WeaponService>> MockLogger { get; set; }

        public int Id { get; }
        public string? Name { get; }
        public Weapon Weapon { get; }
        public Character Character { get; }
        public PaginatedList<Weapon> PaginatedList { get; }

        private Character GetCharacter()
        {
            return new Character()
            {
                Id = Id,
                Name = Name,
                Race = CharacterRace.Human,
                Health = 100,
                CharacterWeapons = GetCharacterWeapons()
            };
        }

        private CharacterWeapon GetCharacterWeapon(int characterId, int weaponId)
        {
            return new CharacterWeapon()
            {
                CharacterId = characterId,
                Character = Character,
                WeaponId = weaponId,
                Weapon = GetWeapon(weaponId)
            };
        }

        private ICollection<CharacterWeapon> GetCharacterWeapons()
        {
            return new List<CharacterWeapon>()
            {
                GetCharacterWeapon(Id, 2),
                GetCharacterWeapon(Id, 3)
            };
        }

        private Weapon GetWeapon(int id)
        {
            return new Weapon()
            {
                Id = id,
                Name = Name,
                Damage = 5,
                Type = WeaponType.Sword
            };
        }

        private List<Weapon> GetWeapons()
        {
            return new List<Weapon>()
            {
                Weapon,
                Weapon
            };
        }

        private PaginatedList<Weapon> GetPaginatedList()
        {
            return new PaginatedList<Weapon>(GetWeapons(), 6, 1, 5);
        }
    }
}
