using FluentValidation;
using Telesecundaria.DTOs.AuthTutor.Request;

namespace Telesecundaria.Validators.AuthTutor
{
    public class AuthTutorCreateRequestValidator : AbstractValidator<LoginTutorRequest>
    {
        public AuthTutorCreateRequestValidator()     
        {

            RuleFor(x => x.Correo)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El correo es obligatorio.")
                .MaximumLength(100).WithMessage("El correo no puede exceder 100 caracteres.")
                .EmailAddress().WithMessage("El formato del correo no es válido.")
                .Matches(@"^[^@]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
                    .WithMessage("El correo debe tener un dominio válido.");

            RuleFor(x => x.Contrasenia)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.")
                .MaximumLength(100).WithMessage("La contraseña no puede exceder 100 caracteres.");



        }


    }
}
