namespace Core.Dtos
{
    public record AddRemoveItemDto
    {
        public int CharacterId { get; init; }
        public int ItemId { get; init; }
    }
}
