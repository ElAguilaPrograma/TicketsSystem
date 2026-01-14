using FluentValidation;
using TicketsSystem.Data.DTOs.TicketsDTO;

namespace TicketsSystem.Core.Validations
{
    public class TicketsDTOValidator : AbstractValidator<TicketsUpdateDto>
    {
        public TicketsDTOValidator()
        {
            RuleFor(t => t.Title).NotEmpty().MaximumLength(50);
            RuleFor(t => t.Description).NotEmpty().MaximumLength(200);
        }
    }
}
