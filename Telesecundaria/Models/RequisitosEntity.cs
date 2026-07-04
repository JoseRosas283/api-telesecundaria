using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class RequisitosEntity
    {
        public string ClaveRequisito { get; set; }
        public string EtapaProceso { get; set; }
        public bool EstadoRequisito { get; set; } = true;
        public string FormatoExigido { get; set; }
        public string ClaveTipoDocumento { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual TipoDocumentosEntity TipoDocumento { get; set; }
    }
}
