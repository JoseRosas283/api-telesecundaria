using FluentValidation;
using Telesecundaria.DTOs.Empleados.Request;

namespace Telesecundaria.Validators.Empleados
{
    public class EmpleadoUpdateRequestValidator : AbstractValidator<EmpleadoUpdateRequest>
    {

        public EmpleadoUpdateRequestValidator()
        {
            RuleFor(x => x.TipoContrato)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("El tipo de contrato es obligatorio.")
            .Must(SeTipoContratoValido)
                .WithMessage("El tipo de contrato debe ser 'Planta' o 'Temporal'.");

            RuleFor(x => x.Telefono)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El teléfono es obligatorio.")
                .MinimumLength(10).WithMessage("El teléfono debe tener al menos 10 dígitos.")
                .MaximumLength(15).WithMessage("El teléfono no puede superar 15 caracteres.")
                .Matches(@"^\d+$").WithMessage("El teléfono solo debe contener números.")
                .Must(NoEmpezarConCero).WithMessage("El teléfono no puede empezar con cero.");
        }

        private bool SeTipoContratoValido(string tipoContrato)
        {
            var tiposValidos = new[] { "Planta", "Temporal" };
            return tiposValidos.Contains(tipoContrato);
        }

        private bool NoEmpezarConCero(string telefono)
            => !string.IsNullOrEmpty(telefono) && telefono[0] != '0';


    }



}

