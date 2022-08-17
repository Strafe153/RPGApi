using Core.Entities;

namespace Core.ViewModels.WeaponViewModels
{
    public record WeaponReadViewModel : WeaponBaseViewModel
    {
        public int Id { get; init; }
        public IEnumerable<Character>? Characters { get; init; }
    }
}
