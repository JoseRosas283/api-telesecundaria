using FluentValidation;
using Telesecundaria.DTOs.Documentos.Request;

namespace Telesecundaria.Validators.Documentos
{
    public class DocumentosCreateRequestValidator : AbstractValidator<DocumentoCreateRequest>
    {
        private readonly string[] _extensionesValidas = { ".pdf" };

        public DocumentosCreateRequestValidator() 
        {
            RuleFor(x => x.ClaveExpediente)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("La clave del expediente es obligatoria.")
               .Matches(@"^EXPE-\d+$").WithMessage("El formato de la clave del expediente no es válido.");

            RuleFor(x => x.ConstanciaSituacionFiscal)
                .NotNull().WithMessage("La Constancia de Situación Fiscal es obligatoria.")
                .Must(SerPdf).WithMessage("La Constancia de Situación Fiscal debe ser un PDF.");

            RuleFor(x => x.CarnetISSSTe)
                .NotNull().WithMessage("El Carnet del ISSSTE es obligatorio.")
                .Must(SerPdf).WithMessage("El Carnet del ISSSTE debe ser un PDF.");

            RuleFor(x => x.IneIdentificacion)
                .NotNull().WithMessage("La INE o Identificación Oficial es obligatoria.")
                .Must(SerPdf).WithMessage("La INE o Identificación Oficial debe ser un PDF.");

            RuleFor(x => x.TituloProfesional)
                .Must(SerPdf).WithMessage("El Título Profesional debe ser un PDF.")
                .When(x => x.TituloProfesional != null);

            RuleFor(x => x.CedulaProfesional)
                .Must(SerPdf).WithMessage("La Cédula Profesional debe ser un PDF.")
                .When(x => x.CedulaProfesional != null);

        }

        private bool SerPdf(IFormFile? archivo)  => archivo != null &&
           Path.GetExtension(archivo.FileName).ToLowerInvariant() == ".pdf";
    }
}
