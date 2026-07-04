namespace Telesecundaria.DTOs.PermisosGestion.Request
{
    public class PermisoGestionarRequest
    {
        public string NombreRol { get; set; }
        public string NombreModulo { get; set; }
        public string PuedeVer { get; set; }        // "Puede" o "No puede" (obligatorio)
        public string? PuedeCrear { get; set; }     // "Puede", "No puede" o null
        public string? PuedeEditar { get; set; }    // "Puede", "No puede" o null
        public string? PuedeEliminar { get; set; }  // "Puede", "No puede" o null
    }
}
