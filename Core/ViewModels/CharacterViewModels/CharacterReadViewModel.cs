using Core.Entities;

namespace Core.ViewModels.CharacterViewModels
{
    public record CharacterReadViewModel : CharacterBaseViewModel
    {
        public int Id { get; init; }
        public int Health { get; init; }

        public int PlayerId { get; init; }

        public IEnumerable<Weapon>? Weapons { get; init; }
        public IEnumerable<Spell>? Spells { get; init; }
        public IEnumerable<Mount>? Mounts { get; init; }
    }
}
