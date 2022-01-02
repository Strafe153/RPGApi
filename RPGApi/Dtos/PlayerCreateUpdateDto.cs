using System.ComponentModel.DataAnnotations;

namespace RPGApi.Dtos
{
    public record PlayerCreateUpdateDto
    {
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string Name { get; init; }
    }
}
