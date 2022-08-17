namespace Core.ViewModels
{
    public record HitViewModel
    {
        public int DealerId { get; init; }
        public int ReceiverId { get; init; }
        public int ItemId { get; init; }
    }
}
