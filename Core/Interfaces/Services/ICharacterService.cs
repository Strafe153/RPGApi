using Core.Entities;

namespace Core.Interfaces.Services
{
    public interface ICharacterService : IService<Character>
    {
        Weapon GetWeapon(Character entity, int spellId);
        Spell GetSpell(Character entity, int spellId);
        void CalculateHealth(Character character, int damage);
    }
}
