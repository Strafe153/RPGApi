using System.ComponentModel.DataAnnotations;

namespace RPGApi.Dtos
{
    public record HitDto
    {
        [Required]
        public Guid DealerId { get; init; }

        [Required]
        public Guid ReceiverId { get; init; }

        [Required]
        public Guid ItemId { get; init; }
    }
}