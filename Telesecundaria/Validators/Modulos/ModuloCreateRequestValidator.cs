using FluentValidation;
using Telesecundaria.DTOs.Modulo.Request;

namespace Telesecundaria.Validators.Modulos
{
    public class ModuloCreateRequestValidator :  AbstractValidator <ModuloCreateRequest>
    {
        public ModuloCreateRequestValidator()
        {
            RuleFor(x => x.NombreModulo)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del módulo es obligatorio.")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder 50 caracteres.")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("El nombre solo puede contener letras.");

            RuleFor(x => x.Descripcion)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(200).WithMessage("La descripción no puede exceder 200 caracteres.")
                .When(x => !string.IsNullOrWhiteSpace(x.Descripcion));

            RuleFor(x => x.UrlModulo)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(100).WithMessage("La URL no puede exceder 100 caracteres.")
                .Matches(@"^/[a-zA-Z0-9/_-]*$").WithMessage("La URL debe empezar con '/' y solo contener letras, números, guiones y barras.")
                .When(x => !string.IsNullOrWhiteSpace(x.UrlModulo));
        }

    }
}
