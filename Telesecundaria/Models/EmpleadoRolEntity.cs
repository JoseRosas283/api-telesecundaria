using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class EmpleadoRolEntity
    {
        public string ClaveEmpleado { get; set; }
        public string ClaveRol { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual EmpleadosEntity Empleado { get; set; }
        [JsonIgnore]
        public virtual RolesEntity Rol { get; set; }
    }
}
