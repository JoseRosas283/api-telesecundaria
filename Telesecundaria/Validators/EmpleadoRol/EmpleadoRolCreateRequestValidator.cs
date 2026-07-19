using FluentValidation;
using Telesecundaria.DTOs.EmpleadoRol.Request;

namespace Telesecundaria.Validators.EmpleadoRol
{
    public class EmpleadoRolCreateRequestValidator : AbstractValidator <RolCreateRequest>
    {

        public EmpleadoRolCreateRequestValidator()
        {

            RuleFor(x => x.ClaveEmpleado)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La clave del empleado es obligatoria.")
                .Matches(@"^EMP-\d+$").WithMessage("El formato de la clave del empleado no es válido. Ejemplo: EMP-000001");

            RuleFor(x => x.NombreRol)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("El nombre del rol es obligatorio.")
               .MaximumLength(20).WithMessage("El nombre del rol no puede exceder 20 caracteres.")
               .Must(SerRolValido).WithMessage("El rol debe ser: 'Administrativo', 'Directivo' o 'Docente'.");

    }

        private bool SerRolValido(string nombreRol)
        {
            var rolesValidos = new[] { "Administrativo", "Directivo", "Docente", "Intendente" };
            return rolesValidos.Contains(nombreRol);
        }


    }
}
