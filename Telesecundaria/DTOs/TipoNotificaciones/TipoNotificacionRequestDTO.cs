using System.ComponentModel.DataAnnotations;

namespace Telesecundaria.DTOs.TipoNotificaciones
{
    public class TipoNotificacionRequestDTO
    {
        [Required] public string NombreProceso { get; set; } = string.Empty;
        [Required] public string Descripcion { get; set; } = string.Empty;
        [Required] public string Icono { get; set; } = string.Empty;
        [Required] public string Color { get; set; } = string.Empty;
    }
}
