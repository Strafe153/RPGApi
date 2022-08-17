using Core.Enums;

namespace Core.ViewModels.MountViewModels
{
    public record MountBaseViewModel
    {
        public string? Name { get; init; }
        public MountType Type { get; init; }
        public int Speed { get; init; }
    }
}
