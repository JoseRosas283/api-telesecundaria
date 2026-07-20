using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class RutaRechazadaEntity
    {
        public string ClaveRuta { get; set; }
        public string ClaveAdjuncion { get; set; }
        public string ClaveDocAspirante { get; set; }
        public string ClaveRevision { get; set; }
        public string RutaArchivoRechazado { get; set; }
        public DateTime? FechaRegistro { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual AdjuncionesEntity Adjuncion { get; set; }
        [JsonIgnore]
        public virtual DocumentosAspiranteEntity DocumentoAspirante { get; set; }
        [JsonIgnore]
        public virtual RevisionesEntity Revision { get; set; }
    }
}
