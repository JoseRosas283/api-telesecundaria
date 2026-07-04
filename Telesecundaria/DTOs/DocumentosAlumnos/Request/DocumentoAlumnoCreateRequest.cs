using System.ComponentModel.DataAnnotations;

namespace Telesecundaria.DTOs.DocumentosAlumnos.Request
{
    public class DocumentoAlumnoCreateRequest
    {
        public string ClaveExpediente { get; set; } = string.Empty;

        [Required] public IFormFile CertificadoPrimaria { get; set; } = null!;
        [Required] public IFormFile CartaBuenaConducta { get; set; } = null!;

        public string ClaveUsuario { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonIgnore]
        internal string ArchivoUrl { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonIgnore]
        internal string NombreTipoDocumento { get; set; } = string.Empty;
    }
}
