using System.ComponentModel.DataAnnotations;

namespace Telesecundaria.DTOs.Documentos.Request
{
    public class DocumentoIntendenteCreateRequest
    {
        public string ClaveExpediente { get; set; } = string.Empty;
        [Required] public IFormFile ConstanciaSituacionFiscal { get; set; } = null!;
        [Required] public IFormFile CarnetISSSTe { get; set; } = null!;
        [Required] public IFormFile IneIdentificacion { get; set; } = null!;

        [System.Text.Json.Serialization.JsonIgnore]
        internal string ClaveUsuario { get; set; } = string.Empty;
        [System.Text.Json.Serialization.JsonIgnore]
        internal string ArchivoUrl { get; set; } = string.Empty;
        [System.Text.Json.Serialization.JsonIgnore]
        internal string NombreTipoDocumento { get; set; } = string.Empty;



    }
}
