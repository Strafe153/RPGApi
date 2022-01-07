namespace RPGApi.Dtos
{
    public record CharacterAddRemoveItem
    {
        public Guid CharacterId { get; init; }
        public Guid ItemId { get; init; }
    }
}
