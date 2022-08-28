using Core.Entities;
using Core.Enums;

namespace Core.ViewModels.PlayerViewModels
{
    public record PlayerReadViewModel : PlayerBaseViewModel
    {
        public int Id { get; init; }
        public PlayerRole Role { get; init; }
        public IEnumerable<Character>? Characters { get; init; }
    }
}
