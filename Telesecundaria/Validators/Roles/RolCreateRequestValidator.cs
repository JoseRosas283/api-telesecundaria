using FluentValidation;
using Telesecundaria.DTOs.EmpleadoRol.Request;
using Telesecundaria.DTOs.Roles.Request;

namespace Telesecundaria.Validators.Roles
{
    public class RolCreateRequestValidator : AbstractValidator<RolesCreateRequest>
    {

        public RolCreateRequestValidator() 
        {

            RuleFor(x => x.NombreRol)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("El nombre del rol es obligatorio.")
            .MinimumLength(3).WithMessage("El nombre del rol debe tener al menos 3 caracteres.")
            .MaximumLength(20).WithMessage("El nombre del rol no puede exceder 20 caracteres.")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("El nombre del rol solo puede contener letras.");

            RuleFor(x => x.Descripcion)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MinimumLength(10).WithMessage("La descripción debe tener al menos 10 caracteres.")
                .MaximumLength(200).WithMessage("La descripción no puede exceder 200 caracteres.");

        }

    }
}
