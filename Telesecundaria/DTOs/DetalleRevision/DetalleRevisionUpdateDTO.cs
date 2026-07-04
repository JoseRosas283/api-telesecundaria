namespace Telesecundaria.DTOs.DetalleRevision
{
    public class DetalleRevisionUpdateDTO
    {
        public string EstatusDoc { get; set; } = string.Empty; // 'Aceptado','Rechazado'
        public string? MotivoRechazo { get; set; }
    }
}
