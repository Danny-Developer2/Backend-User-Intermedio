using FluentValidation;
using prueba.Dto;

namespace prueba.Validators
{
    public class LoginValidator : AbstractValidator<LoginDTO>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("El email es requerido")
                .EmailAddress().WithMessage("El formato del email no es válido");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
                .Matches("[A-Z]").WithMessage("La contraseña debe tener al menos una mayúscula")
                .Matches("[a-z]").WithMessage("La contraseña debe tener al menos una minúscula")
                .Matches("[0-9]").WithMessage("La contraseña debe tener al menos un número")
                .Matches("[^a-zA-Z0-9]").WithMessage("La contraseña debe tener al menos un símbolo");
        }
    }
}