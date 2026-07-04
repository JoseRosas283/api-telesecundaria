namespace Telesecundaria.DTOs.Pagos
{
    public class PagoRequestDTO
    {
        public string ClaveTutor { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = string.Empty; // 'Efectivo','Transferencia','Deposito'
        public string? ComprobantePago { get; set; }
    }
}
