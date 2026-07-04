namespace Telesecundaria.DTOs.DetalleRevision
{
    public class DetalleRevisionResponseDTO
    {
        public string ClaveRevision { get; set; } = string.Empty;
        public string ClaveDocAspirante { get; set; } = string.Empty;
        public string EstatusDoc { get; set; } = string.Empty;
        public string? MotivoRechazo { get; set; }
    }
}
