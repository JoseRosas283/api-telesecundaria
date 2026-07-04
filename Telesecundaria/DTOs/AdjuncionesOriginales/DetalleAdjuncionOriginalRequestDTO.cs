using System.ComponentModel.DataAnnotations;

namespace Telesecundaria.DTOs.AdjuncionesOriginales
{
    public class DetalleAdjuncionOriginalRequestDTO
    {
        [Required] public string ClaveAdjOriginal { get; set; } = string.Empty;
        [Required] public string ClaveDocAspirante { get; set; } = string.Empty;
    }
}
