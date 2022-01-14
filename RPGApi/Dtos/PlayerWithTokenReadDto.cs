namespace RPGApi.Dtos
{
    public record PlayerWithTokenReadDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Token { get; init; }

        public ICollection<Character> Characters { get; init; }
    }
}
