using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.CharacterDtos;

public record ManageItemDto(
	[Required] int CharacterId,
	[Required] int ItemId,
	[Required] ItemType ItemType,
	[Required] ManageItemOperation Operation);