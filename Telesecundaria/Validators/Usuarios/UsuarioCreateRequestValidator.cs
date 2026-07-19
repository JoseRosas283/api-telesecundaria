using FluentValidation;
using Telesecundaria.DTOs.Usuarios.Request;

namespace Telesecundaria.Validators.Usuarios
{
    public class UsuarioCreateRequestValidator : AbstractValidator<UsuarioCreateRequest>
    {
        public UsuarioCreateRequestValidator() 
        {

            RuleFor(x => x.NombreUsuario)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
                .MinimumLength(4).WithMessage("El nombre de usuario debe tener al menos 4 caracteres.")
                .MaximumLength(20).WithMessage("El nombre de usuario no puede exceder 20 caracteres.")
                .Matches(@"^[a-zA-Z0-9._]+$")
                    .WithMessage("El nombre de usuario solo puede contener letras, números, puntos y guiones bajos.");

            RuleFor(x => x.Contrasenia)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .MaximumLength(100).WithMessage("La contraseña no puede exceder 100 caracteres.")
                .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula.")
                .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una minúscula.")
                .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número.")
                .Matches(@"[^a-zA-Z0-9]").WithMessage("La contraseña debe contener al menos un carácter especial.");

            RuleFor(x => x.CorreoInstitucional)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El correo institucional es obligatorio.")
                .MaximumLength(100).WithMessage("El correo no puede exceder 100 caracteres.")
                .EmailAddress().WithMessage("El formato del correo institucional no es válido.")
                .Matches(@"^[^@]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
                    .WithMessage("El correo debe tener un dominio válido.")
                .When(x => !string.IsNullOrWhiteSpace(x.CorreoInstitucional));

            RuleFor(x => x.ClaveEmpleado)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Debe estar ligado a un empleado.")
                .Matches(@"^EMP-\d+$")
                    .WithMessage("El formato de la clave del empleado no es válido. Ejemplo: EMP-000001");


        }


    }
}
