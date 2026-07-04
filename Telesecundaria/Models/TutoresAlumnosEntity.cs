using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class TutoresAlumnosEntity
    {
        public string ClaveAlumno { get; set; }
        public string ClaveTutor { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaBaja { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual AlumnosEntity Alumno { get; set; }
        [JsonIgnore]
        public virtual TutoresEntity Tutor { get; set; }
    }
}
