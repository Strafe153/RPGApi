namespace Core.Dtos
{
    public record HitDto
    {
        public int DealerId { get; init; }
        public int ReceiverId { get; init; }
        public int ItemId { get; init; }
    }
}
