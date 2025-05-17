using Domain.Entities;
using Domain.Enums;

namespace Application.Dtos.MountDtos;

public record MountReadDto(
    int Id,
    string Name,
    MountType Type,
    int Speed,
    IEnumerable<Character> Characters);
