using System.ComponentModel.DataAnnotations;
using RPGApi.Data;

namespace RPGApi.Dtos.Players
{
    public record PlayerChangeRoleDto
    {
        [Required]
        public Guid Id { get; init; }

        public PlayerRole Role { get; init; } = PlayerRole.Player;
    }
}
