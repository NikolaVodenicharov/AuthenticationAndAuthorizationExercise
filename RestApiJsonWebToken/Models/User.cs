﻿using System.Security.Claims;

namespace RestApiJsonWebToken.Models
{
    public class User
    {
        public string Username { get; set; } = string.Empty;

        public byte[]? PasswordHash { get; set; }

        public byte[]? PasswordSalt { get; set; }

        public ICollection<Claim>? Claims { get; set; }

        public string? RefreshTokenString { get; set; }

        public DateTime? RefreshTokenExpirationDate { get; set; }
    }
}
