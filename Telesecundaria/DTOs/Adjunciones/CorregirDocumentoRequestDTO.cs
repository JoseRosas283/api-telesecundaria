using System.ComponentModel.DataAnnotations;

namespace Telesecundaria.DTOs.Adjunciones
{
    public class CorregirDocumentoRequestDTO
    {
        [Required] public IFormFile Archivo { get; set; } = null!;
    }
}
