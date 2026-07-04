using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class EnviosEntity
    {
        public string ClaveEnvio { get; set; }
        public string ClaveNotificacion { get; set; }
        public string Destino { get; set; }
        public int ReintentoNum { get; set; } = 0;
        public string Estatus { get; set; } = "Pendiente";
        public bool ConfirmacionLectura { get; set; } = false;
        public DateTime? FechaEnvio { get; set; }
        public string? ErrorLog { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual NotificacionesEntity Notificacion { get; set; }
    }
}
