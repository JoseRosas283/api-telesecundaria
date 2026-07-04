using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class ValidacionDocumentosEntity
    {
        public string ClaveEntrega { get; set; }
        public string ClaveDocAspirante { get; set; }
        public string EstatusCotejo { get; set; }
        public DateTime FechaValidacion { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual EntregasEntity Entrega { get; set; }
        [JsonIgnore]
        public virtual DocumentosAspiranteEntity DocumentoAspirante { get; set; }
    }
}
