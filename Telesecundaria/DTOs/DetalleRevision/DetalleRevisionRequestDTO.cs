namespace Telesecundaria.DTOs.DetalleRevision
{
    public class DetalleRevisionRequestDTO
    {
        public string ClaveRevision { get; set; } = string.Empty;
        public string ClaveDocAspirante { get; set; } = string.Empty;
        public string EstatusDoc { get; set; } = string.Empty; // 'Aceptado','Rechazado'
        public string? MotivoRechazo { get; set; }
    }
}
