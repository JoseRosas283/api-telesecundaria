using System.ComponentModel.DataAnnotations;

namespace Telesecundaria.DTOs.AdjuncionesOriginales
{
    public class AdjuncionOriginalRequestDTO
    {
        [Required] public string ClaveEntrega { get; set; } = string.Empty;
        [Required] public string ClaveUsuario { get; set; } = string.Empty;
    }
}
