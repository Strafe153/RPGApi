namespace Core.VeiwModels.CharacterViewModels
{
    public record CharacterCreateViewModel : CharacterBaseViewModel
    {
        public int PlayerId { get; init; }
    }
}
