using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Dtos.CharactersDtos;

public record ManageItemDto(
    [Required] int CharacterId,
    [Required] int ItemId,
    [Required] ItemType ItemType,
    [Required] ManageItemOperation Operation);