namespace Telesecundaria.DTOs.TutoresAlumnos
{
    public class TutorAlumnoResponseDTO
    {
        public string ClaveAlumno { get; set; } = string.Empty;
        public string ClaveTutor { get; set; } = string.Empty;
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
