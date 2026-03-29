using System;
using System.Collections.Generic;
using System.Text;

namespace SecretVault.Application.DTOs
{
    public class RegisterResponseDto
    {
        public Guid UserId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
