using RPGApi.Dtos;

namespace RPGApi.Models
{
    public record PlayersPageDto
    {
        public IEnumerable<PlayerReadDto>? PlayerDtos { get; init; }
        public int PagesCount { get; init; }
        public int CurrentPage { get; init; }
    }
}
