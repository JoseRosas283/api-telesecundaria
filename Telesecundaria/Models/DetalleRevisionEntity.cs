using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class DetalleRevisionEntity
    {
        public string ClaveRevision { get; set; }
        public string ClaveDocAspirante { get; set; }
        public string EstatusDoc { get; set; }
        public string? MotivoRechazo { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual RevisionesEntity Revision { get; set; }
        [JsonIgnore]
        public virtual DocumentosAspiranteEntity DocumentoAspirante { get; set; }
    }
}
