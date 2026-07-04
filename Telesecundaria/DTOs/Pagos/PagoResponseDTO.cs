namespace Telesecundaria.DTOs.Pagos
{
    public class PagoResponseDTO
    {
        public string ClavePago { get; set; } = string.Empty;
        public string ClaveTutor { get; set; } = string.Empty;
        public string ClaveCiclo { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime? FechaPago { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string? ComprobantePago { get; set; }
        public string? Referencia { get; set; }
        public bool? EstadoPago { get; set; }
    }
}
