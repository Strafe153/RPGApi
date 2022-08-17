using Core.Enums;

namespace Core.ViewModels.WeaponViewModels
{
    public record WeaponBaseViewModel
    {
        public string? Name { get; init; }
        public WeaponType Type { get; init; }
        public int Damage { get; init; } = 15;
    }
}
