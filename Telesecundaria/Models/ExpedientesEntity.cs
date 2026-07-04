using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class ExpedientesEntity
    {
        public string ClaveExpediente { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Curp { get; set; }
        public string TipoTitular { get; set; }
        public string ClaveEntrega { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual EntregasEntity Entrega { get; set; }

        // Colecciones
        public ICollection<DocumentosEntity> Documentos { get; set; } = new List<DocumentosEntity>();
    }
}
