using FluentValidation;
using Telesecundaria.DTOs.Documentos.Request;
using Telesecundaria.DTOs.DocumentosAlumnos.Request;

namespace Telesecundaria.Validators.Documentos
{
    public class DocumentoAlumnoCreateRequestValidator : AbstractValidator<DocumentoAlumnoCreateRequest>
    {

        public DocumentoAlumnoCreateRequestValidator()
        {

            RuleFor(x => x.ClaveExpediente)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La clave del expediente es obligatoria.")
                .Matches(@"^EXPE-\d+$")
                    .WithMessage("El formato de la clave del expediente no es válido.");

            RuleFor(x => x.CertificadoPrimaria)
                .NotNull().WithMessage("El Certificado de Primaria es obligatorio.")
                .Must(SerPdf).WithMessage("El Certificado de Primaria debe ser un PDF.");

            RuleFor(x => x.CartaBuenaConducta)
                .NotNull().WithMessage("La Carta de Buena Conducta es obligatoria.")
                .Must(SerPdf).WithMessage("La Carta de Buena Conducta debe ser un PDF.");



        }

        private bool SerPdf(IFormFile? archivo) => archivo != null && Path.GetExtension(archivo.FileName).ToLowerInvariant() == ".pdf";

    }
}
