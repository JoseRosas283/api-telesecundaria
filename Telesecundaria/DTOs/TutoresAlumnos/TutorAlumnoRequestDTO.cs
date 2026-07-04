using System.ComponentModel.DataAnnotations;

namespace Telesecundaria.DTOs.TutoresAlumnos
{
    public class TutorAlumnoRequestDTO
    {
        [Required] public string ClaveAlumno { get; set; } = string.Empty;
        [Required] public string ClaveTutor { get; set; } = string.Empty;
    }
}
