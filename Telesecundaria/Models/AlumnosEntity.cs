using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class AlumnosEntity
    {
        public string ClaveAlumno { get; set; }
        public string? Matricula { get; set; }
        public string? Estado { get; set; }
        public string ClaveExpediente { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual ExpedientesEntity Expediente { get; set; }

        // Colecciones
        public ICollection<TutoresAlumnosEntity> TutoresAlumnos { get; set; } = new List<TutoresAlumnosEntity>();
        public ICollection<AsignacionGrupoEntity> AsignacionGrupos { get; set; } = new List<AsignacionGrupoEntity>();
    }
}
