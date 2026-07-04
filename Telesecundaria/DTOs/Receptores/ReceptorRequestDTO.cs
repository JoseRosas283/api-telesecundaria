using System.ComponentModel.DataAnnotations;

namespace Telesecundaria.DTOs.Receptores
{
    public class ReceptorRequestDTO
    {
        [Required] public string TipoReceptor { get; set; } = string.Empty; // 'TutorAspirante','Tutor','Usuario'
        [Required] public string ClaveReferencia { get; set; } = string.Empty; // clave del Tutor/TutorAspirante/Usuario
        [Required] public string Correo { get; set; } = string.Empty;
    }
}
