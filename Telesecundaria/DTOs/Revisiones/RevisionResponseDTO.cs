namespace Telesecundaria.DTOs.Revisiones
{
    public class RevisionResponseDTO
    {
        public string ClaveRevision { get; set; } = string.Empty;
        public string ClaveAdjuncion { get; set; } = string.Empty;
        public string ClaveUsuario { get; set; } = string.Empty;
        public string EstatusRevision { get; set; } = string.Empty;
        public string EstadoOperativo { get; set; } = string.Empty;
        public string? ObservacionGeneral { get; set; }
        public DateTime? FechaRevision { get; set; }
    }
}
