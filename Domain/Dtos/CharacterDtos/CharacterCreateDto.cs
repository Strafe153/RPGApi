namespace Domain.Dtos.CharacterDtos;

public record CharacterCreateDto : CharacterBaseDto
{
    public int PlayerId { get; init; }
}
