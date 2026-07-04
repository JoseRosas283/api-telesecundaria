using System.ComponentModel.DataAnnotations;

namespace Telesecundaria.DTOs.Tutores
{
    public class TutorRequestDTO
    {
        [Required] public string Nombre { get; set; } = string.Empty;
        [Required] public string ApellidoPaterno { get; set; } = string.Empty;
        public string? ApellidoMaterno { get; set; }
        [Required] public string CurpTutor { get; set; } = string.Empty;
        [Required] public string Telefono { get; set; } = string.Empty;
        [Required] public string Correo { get; set; } = string.Empty;
        [Required] public string Parentesco { get; set; } = string.Empty;
    }
}
