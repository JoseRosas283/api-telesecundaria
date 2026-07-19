using FluentValidation;
using Telesecundaria.DTOs.Empleados.Request;

namespace Telesecundaria.Validators.Empleados
{
    public class EmpleadoCreateRequestValidator : AbstractValidator<EmpleadoCreateRequest>
    {

        public EmpleadoCreateRequestValidator() 
        {

            RuleFor(x => x.ClaveExpediente)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("La clave de expediente es obligatoria.")
               .Matches(@"^EXPE-\d+$").WithMessage("El formato de la clave de expediente no es válido. Ejemplo: EXPE-0000000000001");

            RuleFor(x => x.TipoContrato)
                .NotEmpty().WithMessage("El tipo de contrato es obligatorio.")
                .Must(SeTipoContratoValido).WithMessage("El tipo de contrato debe ser 'Planta' o 'Temporal'.");

            RuleFor(x => x.Telefono)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El teléfono es obligatorio.")
                .MinimumLength(10).WithMessage("El teléfono debe tener al menos 10 dígitos.")
                .MaximumLength(15).WithMessage("El teléfono no puede superar 15 caracteres.")
                .Matches(@"^\d+$").WithMessage("El teléfono solo debe contener números.")
                .Must(NoEmpezarConCeros).WithMessage("El teléfono no puede empezar con cero.");

            RuleFor(x => x.FechaContratacion)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La fecha de contratación es obligatoria.")
                .GreaterThan(new DateTime(2023, 1, 1)).WithMessage("La fecha de contratación no puede ser anterior al año 2023.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de contratación no puede ser futura.");
        }

        private bool SeTipoContratoValido(string tipoContrato)
        {
            var tiposValidos = new[] { "Planta", "Temporal" };
            return tiposValidos.Contains(tipoContrato);
        }


        private bool NoEmpezarConCeros(string telefono)
        {
            if (string.IsNullOrEmpty(telefono)) return false;
            return telefono[0] != '0';
        }

    }
}
