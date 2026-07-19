namespace Telesecundaria.DTOs.Entregas
{
    public class EntregaResponseDTO
    {
        public string ClaveEntrega { get; set; } = string.Empty;
        public string EstadoFinal { get; set; } = string.Empty;
        public string ClaveCita { get; set; } = string.Empty;
        public string ClaveAspirante { get; set; } = string.Empty;
        public string ClaveTutorAspirante { get; set; } = string.Empty;
        public string ClaveUsuario { get; set; } = string.Empty;
        public DateTime? FechaFormalizacion { get; set; }
    }
}
