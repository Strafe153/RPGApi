using System.ComponentModel.DataAnnotations;

namespace RPGApi.Dtos
{
    public record PlayerAuthorizeDto
    {
        [Required]
        public string Name { get; init; }

        [Required]
        public string Password { get; init; }
    }
}
