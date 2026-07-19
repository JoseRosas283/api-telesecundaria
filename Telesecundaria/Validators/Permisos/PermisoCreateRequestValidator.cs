using FluentValidation;
using Telesecundaria.DTOs.PermisosGestion.Request;

namespace Telesecundaria.Validators.Permisos
{
    public class PermisoCreateRequestValidator  : AbstractValidator<PermisoGestionarRequest>
    {
        private readonly string[] _valoresPermitidos = { "Puede", "No puede" };

        public PermisoCreateRequestValidator() 
        {
            RuleFor(x => x.NombreRol)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("El nombre del rol es obligatorio.")
               .MaximumLength(20).WithMessage("El nombre del rol no puede exceder 20 caracteres.");

            RuleFor(x => x.NombreModulo)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del módulo es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre del módulo no puede exceder 50 caracteres.");

            RuleFor(x => x.PuedeVer)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El permiso 'PuedeVer' es obligatorio.")
                .Must(SerPermisoValido)
                    .WithMessage("PuedeVer debe ser 'Puede' o 'No puede'.");

            RuleFor(x => x.PuedeCrear)
                .Must(SerPermisoValido).WithMessage("PuedeCrear debe ser 'Puede' o 'No puede'.")
                .When(x => !string.IsNullOrWhiteSpace(x.PuedeCrear));

            RuleFor(x => x.PuedeEditar)
                .Must(SerPermisoValido).WithMessage("PuedeEditar debe ser 'Puede' o 'No puede'.")
                .When(x => !string.IsNullOrWhiteSpace(x.PuedeEditar));

            RuleFor(x => x.PuedeEliminar)
                .Must(SerPermisoValido).WithMessage("PuedeEliminar debe ser 'Puede' o 'No puede'.")
                .When(x => !string.IsNullOrWhiteSpace(x.PuedeEliminar));


        }
        private bool SerPermisoValido(string? permiso)=> _valoresPermitidos.Contains(permiso);
    }
}
