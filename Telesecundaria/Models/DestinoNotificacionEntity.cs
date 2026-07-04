using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class DestinoNotificacionEntity
    {
        public string ClaveDestino { get; set; }
        public string ClaveTipoNotificacion { get; set; }
        public string TipoReceptor { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual TipoNotificacionesEntity TipoNotificacion { get; set; }
    }
}
