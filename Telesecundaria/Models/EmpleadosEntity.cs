using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class EmpleadosEntity
    {
        public string ClaveEmpleado { get; set; }
        public DateTime FechaContratacion { get; set; }
        public string TipoContrato { get; set; }
        public string EstatusLaboral { get; set; }
        public string Telefono { get; set; }
        public string ClaveExpediente { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual ExpedientesEntity Expediente { get; set; }

        // Colecciones
        public ICollection<EmpleadoRolEntity> EmpleadoRoles { get; set; } = new List<EmpleadoRolEntity>();

        // Relación 1:1
        public UsuariosEntity? Usuario { get; set; }
    }
}
