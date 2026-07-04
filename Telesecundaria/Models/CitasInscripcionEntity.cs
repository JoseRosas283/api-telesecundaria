using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class CitasInscripcionEntity
    {
        public string ClaveCita { get; set; }
        public DateTime FechaCita { get; set; }
        public TimeSpan HoraCita { get; set; }
        public string EstadoCita { get; set; } = "Programada";
        public string? Observaciones { get; set; }
        public DateTime CreateAt { get; set; }
        public string ClaveRevision { get; set; }
        public string ClaveTutorAspirante { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual RevisionesEntity Revision { get; set; }
        [JsonIgnore]
        public virtual TutorAspiranteEntity TutorAspirante { get; set; }

        // Relación 1:1
        public EntregasEntity Entrega { get; set; }
    }
}
