﻿using System.ComponentModel.DataAnnotations;

namespace RPGApi.Dtos.Players
{
    public record PlayerUpdateDto
    {
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string? Name { get; init; }
    }
}
