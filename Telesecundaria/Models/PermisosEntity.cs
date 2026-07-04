namespace Telesecundaria.Models
{
    public class PermisosEntity
    {
        // PK compuesta: (ClaveRol, ClaveModulo)
        public string ClaveRol { get; set; }
        public string ClaveModulo { get; set; }

        public bool PuedeVer { get; set; } = true;
        public bool PuedeCrear { get; set; } = false;
        public bool PuedeEditar { get; set; } = false;
        public bool PuedeEliminar { get; set; } = false;

        public DateTime FechaAsignacion { get; set; }

        // Navegación
        public RolesEntity Rol { get; set; }
        public ModulosEntity Modulo { get; set; }
    }
}
