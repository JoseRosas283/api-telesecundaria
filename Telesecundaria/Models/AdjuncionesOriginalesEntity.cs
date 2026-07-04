using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class AdjuncionesOriginalesEntity
    {
        public string ClaveAdjOriginal { get; set; }
        public string ClaveEntrega { get; set; }
        public string ClaveUsuario { get; set; }
        public DateTime? FechaCarga { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual EntregasEntity Entrega { get; set; }
        [JsonIgnore]
        public virtual UsuariosEntity Usuario { get; set; }

        // Colecciones
        public ICollection<DetalleAdjuncionOriginalEntity> Detalles { get; set; } = new List<DetalleAdjuncionOriginalEntity>();
    }
}
