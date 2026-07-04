using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class AdjuncionesEntity
    {
        public string ClaveAdjuncion { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string EstatusGral { get; set; }
        public string EstatusOperativo { get; set; } = "Abierta";
        public string ClaveTutorAspirante { get; set; }
        public string ClaveAspirante { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual TutorAspiranteEntity TutorAspirante { get; set; }
        [JsonIgnore]
        public virtual AspirantesEntity Aspirante { get; set; }

        // Colecciones
        public ICollection<DetalleAdjuncionEntity> DetalleAdjuncion { get; set; } = new List<DetalleAdjuncionEntity>();

        // Relación 1:1
        public RevisionesEntity Revision { get; set; }
    }
}
