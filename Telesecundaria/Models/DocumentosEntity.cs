using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class DocumentosEntity
    {
        public string ClaveDocumento { get; set; }
        public string ArchivoUrl { get; set; }
        public string Estado { get; set; }
        public DateTime FechaSubida { get; set; }
        public string ClaveExpediente { get; set; }
        public string ClaveTipoDocumento { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual ExpedientesEntity Expediente { get; set; }
        [JsonIgnore]
        public virtual TipoDocumentosEntity TipoDocumento { get; set; }
    }
}
