using Core.Enums;

namespace Core.ViewModels.CharacterViewModels
{
    public record CharacterBaseViewModel
    {
        public string? Name { get; init; }
        public CharacterRace Race { get; init; }
    }
}
