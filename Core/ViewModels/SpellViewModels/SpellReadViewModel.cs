using Core.Entities;

namespace Core.ViewModels.SpellViewModels
{
    public record SpellReadViewModel : SpellBaseViewModel
    {
        public int Id { get; init; }
        public IEnumerable<Character>? Characters { get; init; }
    }
}
