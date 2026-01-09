using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using TicketsSystem.Data.DTOs;

namespace TicketsSystem.Core.Validations
{
    public class LoginRequestValidation : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidation()
        {
            RuleFor(l => l.Email).NotEmpty().EmailAddress();
            RuleFor(l => l.Password).NotEmpty();
        }
    }
}
