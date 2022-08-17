using Core.Entities;

namespace Core.ViewModels.MountViewModels
{
    public record MountReadViewModel : MountBaseViewModel
    {
        public int Id { get; init; }
        public IEnumerable<Character>? Characters { get; init; }
    }
}
