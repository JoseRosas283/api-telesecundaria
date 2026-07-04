using System.ComponentModel.DataAnnotations;

namespace Telesecundaria.DTOs.Adjunciones
{
    public class DocumentoTempRequestDTO
    {
        [Required] public string ClaveAspirante { get; set; } = string.Empty;
        [Required] public string TipoDocumento { get; set; } = string.Empty;
        [Required] public IFormFile Archivo { get; set; } = null!;
    }
}
