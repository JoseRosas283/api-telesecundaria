using FluentValidation;
using Telesecundaria.DTOs.Expedientes.Request;

namespace Telesecundaria.Validators.Expedientes
{
    public class ExpedienteUpdateRequestValidator : AbstractValidator<ExpedienteUpdateRequest>
    {
        public ExpedienteUpdateRequestValidator() 
        {

            RuleFor(x => x.Nombre)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("El nombre es obligatorio.")
               .MinimumLength(2).WithMessage("El nombre debe tener al menos 2 caracteres.")
               .MaximumLength(30).WithMessage("El nombre no puede exceder 30 caracteres.")
               .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$").WithMessage("El nombre solo puede contener letras.");

            RuleFor(x => x.ApellidoPaterno)
                .NotEmpty().WithMessage("El apellido paterno es obligatorio.")
                .MinimumLength(2).WithMessage("El apellido paterno debe tener al menos 2 caracteres.")
                .MaximumLength(30).WithMessage("El apellido paterno no puede exceder 30 caracteres.")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$").WithMessage("El apellido paterno solo puede contener letras.")
                .NotEqual(r => r.Nombre).WithMessage("El apellido paterno no puede ser igual al nombre.");

            RuleFor(x => x.ApellidoMaterno)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(50).WithMessage("El apellido materno no puede exceder 50 caracteres.")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$").WithMessage("El apellido materno solo puede contener letras.")
                .NotEqual(r => r.Nombre).WithMessage("El apellido materno no puede ser igual al nombre.")
                .When(x => !string.IsNullOrWhiteSpace(x.ApellidoMaterno));

            RuleFor(x => x.Curp)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La CURP es obligatoria.")
                .Length(18).WithMessage("La CURP debe tener exactamente 18 caracteres.")
                .Matches(@"^[A-Z]{4}\d{6}[HM][A-Z]{5}[A-Z0-9]\d$").WithMessage("El formato de la CURP no es válido.")
                .Must(SinTenerEspacios).WithMessage("La CURP no puede contener espacios.");



        }

        private bool SinTenerEspacios(string valor)
        {
            return !valor.Contains(" ");
        }

    }
}
