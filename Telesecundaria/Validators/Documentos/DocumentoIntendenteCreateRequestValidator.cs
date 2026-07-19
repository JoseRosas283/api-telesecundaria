using FluentValidation;
using Telesecundaria.DTOs.Documentos.Request;

namespace Telesecundaria.Validators.Documentos
{
    public class DocumentoIntendenteCreateRequestValidator : AbstractValidator<DocumentoIntendenteCreateRequest>
    {

        public DocumentoIntendenteCreateRequestValidator()
        {
            RuleFor(x => x.ClaveExpediente)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("La clave del expediente es obligatoria.")
            .Matches(@"^EXPE-\d+$").WithMessage("El formato de la clave del expediente no es válido.");

            RuleFor(x => x.ConstanciaSituacionFiscal)
                .NotNull().WithMessage("La Constancia de Situación Fiscal es obligatoria.")
                .Must(SerPdf).WithMessage("Debe ser un PDF.");

            RuleFor(x => x.CarnetISSSTe)
                .NotNull().WithMessage("El Carnet del ISSSTE es obligatorio.")
                .Must(SerPdf).WithMessage("Debe ser un PDF.");

            RuleFor(x => x.IneIdentificacion)
                .NotNull().WithMessage("La INE o Identificación Oficial es obligatoria.")
                .Must(SerPdf).WithMessage("Debe ser un PDF.");

        }
        private bool SerPdf(IFormFile? archivo)
        => archivo != null &&
           Path.GetExtension(archivo.FileName).ToLowerInvariant() == ".pdf";



    }
}
