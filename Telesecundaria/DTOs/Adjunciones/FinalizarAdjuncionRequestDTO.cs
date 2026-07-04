using System.ComponentModel.DataAnnotations;

namespace Telesecundaria.DTOs.Adjunciones
{
    public class FinalizarAdjuncionRequestDTO
    {
        [Required] public string ClaveTutor { get; set; } = string.Empty;
        [Required] public string ClaveAspirante { get; set; } = string.Empty;
    }
}
