using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class ReceptoresEntity
    {
        public string ClaveReceptor { get; set; }
        public string TipoReceptor { get; set; }
        public string ClaveTutorAspirante { get; set; }
        public string ClaveTutor { get; set; }
        public string ClaveUsuario { get; set; }
        public string CorreoDestino { get; set; }
        public bool Estado { get; set; } = true;

        // Navegación
        [JsonIgnore]
        public virtual TutorAspiranteEntity TutorAspirante { get; set; }
        [JsonIgnore]
        public virtual TutoresEntity Tutor { get; set; }
        [JsonIgnore]
        public virtual UsuariosEntity Usuario { get; set; }

        // Colecciones
        public ICollection<NotificacionesEntity> Notificaciones { get; set; } = new List<NotificacionesEntity>();
    }
}
