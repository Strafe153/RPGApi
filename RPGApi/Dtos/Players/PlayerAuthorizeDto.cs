using System.ComponentModel.DataAnnotations;

namespace RPGApi.Dtos.Players
{
    public record PlayerAuthorizeDto
    {
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string? Name { get; init; }

        [Required]
        [StringLength(18, MinimumLength = 6)]
        public string? Password { get; init; }
    }
}
