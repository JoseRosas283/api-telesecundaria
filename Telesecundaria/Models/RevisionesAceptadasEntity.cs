using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class RevisionesAceptadasEntity
    {
        public string ClaveRevisionAceptada { get; set; }
        public string ClaveRevision { get; set; }
        public string ClaveReceptor { get; set; }
        public string ClaveConvocatoria { get; set; }
        public DateTime FechaAceptacion { get; set; }
        public bool Estado { get; set; } = true;

        // Navegación
        [JsonIgnore]
        public virtual RevisionesEntity Revision { get; set; }
        [JsonIgnore]
        public virtual ReceptoresEntity Receptor { get; set; }
        [JsonIgnore]
        public virtual ConvocatoriasEntity Convocatoria { get; set; }
    }
}
