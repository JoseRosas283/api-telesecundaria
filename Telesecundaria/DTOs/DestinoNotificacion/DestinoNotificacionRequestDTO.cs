using System.ComponentModel.DataAnnotations;

namespace Telesecundaria.DTOs.DestinoNotificacion
{
    public class DestinoNotificacionRequestDTO
    {
        [Required] public string NombreProceso { get; set; } = string.Empty;
        [Required] public string TipoReceptor { get; set; } = string.Empty; // 'TutorAspirante','Tutor','Usuario'
    }
}
