using FluentValidation;
using prueba.Dto;

namespace prueba.Validators
{
    public class TicketCreateValidator : AbstractValidator<TicketCreateDto>
    {
        public TicketCreateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título es requerido")
                .Length(5, 100).WithMessage("El título debe tener entre 5 y 100 caracteres");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es requerida")
                .Length(10, 1000).WithMessage("La descripción debe tener entre 10 y 1000 caracteres");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("La prioridad seleccionada no es válida");
        }
    }
}
