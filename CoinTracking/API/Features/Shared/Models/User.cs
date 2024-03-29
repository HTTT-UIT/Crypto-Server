﻿using System.ComponentModel.DataAnnotations;

namespace API.Features.Shared.Models
{
    public class User
    {
        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}