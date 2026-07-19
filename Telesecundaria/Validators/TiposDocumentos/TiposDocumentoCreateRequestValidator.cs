using FluentValidation;
using Telesecundaria.DTOs.TipoDocumentos;

namespace Telesecundaria.Validators.TiposDocumentos
{
    public class TiposDocumentoCreateRequestValidator : AbstractValidator <TipoDocumentoRequestDTO>
    {

        public TiposDocumentoCreateRequestValidator() 
        {

            RuleFor(x => x.NombreDocumento)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("El nombre del documento es obligatorio.")
               .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres.")
               .MaximumLength(50).WithMessage("El nombre no puede exceder 50 caracteres.")
               .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
                   .WithMessage("El nombre solo puede contener letras.");

            RuleFor(x => x.Area)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El área es obligatoria.")
                .Must(SerAreaValida)
                    .WithMessage("El área debe ser 'Inscripción', 'Preinscripción' o 'Laboral'.");

            RuleFor(x => x.Descripcion)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MinimumLength(10).WithMessage("La descripción debe tener al menos 10 caracteres.")
                .MaximumLength(200).WithMessage("La descripción no puede exceder 200 caracteres.");

        }

        private bool SerAreaValida(string area)
        {
            var areasValidas = new[] { "Inscripción", "Preinscripción", "Laboral" };
            return areasValidas.Contains(area);
        }


        }
}
