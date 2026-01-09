using System;
using System.Collections.Generic;
using System.Text;

namespace TicketsSystem.Core.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public DateTime? Expiration { get; set; }
    }
}
