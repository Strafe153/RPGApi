using Core.Enums;

namespace Core.ViewModels.SpellViewModels
{
    public record SpellBaseViewModel
    {
        public string? Name { get; init; }
        public SpellType Type { get; init; }
        public int Damage { get; init; } = 12;
    }
}
