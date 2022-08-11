using Core.Enums;

namespace Core.VeiwModels.WeaponViewModels
{
    public record WeaponBaseViewModel
    {
        public string? Name { get; init; }
        public WeaponType Type { get; init; }
        public int Damage { get; init; }
    }
}
