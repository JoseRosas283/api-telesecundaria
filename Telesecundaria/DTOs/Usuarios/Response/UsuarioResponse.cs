namespace Telesecundaria.DTOs.Usuarios.Response
{
    public class UsuarioResponse
    {
        public string ClaveUsuario { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string? CorreoInstitucional { get; set; }
        public bool Estado { get; set; }

        // ── De Empleados (navegación: Usuario → Empleado) ──
        public string ClaveEmpleado { get; set; } = string.Empty;
        public string TipoContrato { get; set; } = string.Empty;
        public string EstatusLaboral { get; set; } = string.Empty;

        // ── De Expedientes (navegación: Empleado → Expediente) ──
        public string NombreCompleto { get; set; } = string.Empty; // nombre + apellidos

        // ── De EmpleadoRol (el rol donde FechaFin IS NULL) ──
        public string RolActual { get; set; } = string.Empty;

        // ── De Receptores ──────────────────────────────
        public string? ClaveReceptor { get; set; }
        public bool ReceptorActivo { get; set; }
    }
}
