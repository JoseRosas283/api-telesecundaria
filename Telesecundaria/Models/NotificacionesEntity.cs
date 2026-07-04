using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class NotificacionesEntity
    {
        public string ClaveNotificacion { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public short Prioridad { get; set; } = 1;
        public string Datos { get; set; }
        public bool Visualizacion { get; set; } = false;
        public DateTime FechaCreacion { get; set; }
        public string ClaveTipoNotificacion { get; set; }
        public string ClaveReceptor { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual TipoNotificacionesEntity TipoNotificacion { get; set; }
        [JsonIgnore]
        public virtual ReceptoresEntity Receptor { get; set; }

        // Colecciones
        public ICollection<EnviosEntity> Envios { get; set; } = new List<EnviosEntity>();
    }
}
