using System.ComponentModel.DataAnnotations;
using RPGApi.Data;

namespace RPGApi.Dtos.Mounts
{
    public record MountCreateUpdateDto
    {
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string? Name { get; init; }

        public MountType Type { get; init; } = MountType.Horse;

        [Range(1, 50)]
        public int Speed { get; init; } = 8;
    }
}
