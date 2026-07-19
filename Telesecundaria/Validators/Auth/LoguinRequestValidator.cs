using FluentValidation;

using Telesecundaria.DTOs.Auth.Request;

namespace Telesecundaria.Validators.Auth
{
    public class LoguinRequestValidator : AbstractValidator<LoginRequest>
    {

        public LoguinRequestValidator() 
        {

            RuleFor(x => x.NombreUsuario)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
                .MinimumLength(4).WithMessage("El nombre de usuario debe tener al menos 4 caracteres.")
                .MaximumLength(30).WithMessage("El nombre de usuario no puede exceder de 30 catacteres.")
                .Matches(@"^[a-zA-Z0-9._]+$").WithMessage("El nombre de usuario solo puede contener letras, números, puntos y guiones bajos.");

            RuleFor(x => x.Contrasenia)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(6).WithMessage("La contraseña debe tener como minimo 6 caracteres.")
                .MaximumLength(100).WithMessage("La contraseña no puede exceder 100 caracteres.");
        }




    }
}
