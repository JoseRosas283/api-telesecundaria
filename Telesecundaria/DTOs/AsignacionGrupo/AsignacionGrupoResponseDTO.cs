namespace Telesecundaria.DTOs.AsignacionGrupo
{
    public class AsignacionGrupoResponseDTO
    {
        public string ClaveAsignacion { get; set; } = string.Empty;
        public string ClaveAlumno { get; set; } = string.Empty;
        public string ClaveGrupo { get; set; } = string.Empty;
        public string ClaveUsuario { get; set; } = string.Empty;
        public string ClaveCiclo { get; set; } = string.Empty;
        public DateTime? FechaAsignacion { get; set; }
        public string? Estatus { get; set; }
    }
}
