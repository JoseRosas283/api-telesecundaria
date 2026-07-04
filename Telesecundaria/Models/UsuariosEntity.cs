using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class UsuariosEntity
    {
        public string ClaveUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Contrasenia { get; set; }
        public string CorreoInstitucional { get; set; }
        public bool Estado { get; set; } = true;
        public string ClaveEmpleado { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual EmpleadosEntity Empleado { get; set; }

        // Colecciones
        public ICollection<PublicacionesEntity> Publicaciones { get; set; } = new List<PublicacionesEntity>();
        public ICollection<RevisionesEntity> Revisiones { get; set; } = new List<RevisionesEntity>();
        public ICollection<ReceptoresEntity> Receptores { get; set; } = new List<ReceptoresEntity>();
        public ICollection<EntregasEntity> Entregas { get; set; } = new List<EntregasEntity>();
        public ICollection<LogueosEntity> Logueos { get; set; } = new List<LogueosEntity>();
    }
}
