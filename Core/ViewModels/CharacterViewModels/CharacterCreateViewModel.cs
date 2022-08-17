namespace Core.ViewModels.CharacterViewModels
{
    public record CharacterCreateViewModel : CharacterBaseViewModel
    {
        public int PlayerId { get; init; }
    }
}
