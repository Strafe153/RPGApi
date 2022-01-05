namespace RPGApi.Dtos
{
    public record PlayerReadDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }

        public ICollection<Character> Characters { get; init; }
    }
}
