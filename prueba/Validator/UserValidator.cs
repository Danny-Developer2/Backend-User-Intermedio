using FluentValidation;
using prueba.Dto;

namespace prueba.Validators
{
    public class UserValidator : AbstractValidator<UserDTO>
    {
        public UserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre es requerido")
                .Length(3, 50).WithMessage("El nombre debe tener entre 3 y 50 caracteres")
                .Matches("^[a-zA-Z]+$").WithMessage("El nombre solo debe contener letras");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El apellido es requerido")
                .Length(3, 50).WithMessage("El apellido debe tener entre 3 y 50 caracteres")
                .Matches("^[a-zA-Z]+$").WithMessage("El apellido solo debe contener letras");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es requerido")
                .EmailAddress().WithMessage("El formato del email no es válido");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("El teléfono es requerido")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("El formato del teléfono no es válido");
        }
    }
}