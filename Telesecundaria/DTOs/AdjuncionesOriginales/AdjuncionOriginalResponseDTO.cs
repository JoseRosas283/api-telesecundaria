namespace Telesecundaria.DTOs.AdjuncionesOriginales
{
    public class AdjuncionOriginalResponseDTO
    {
        public string ClaveAdjOriginal { get; set; } = string.Empty;
        public string ClaveEntrega { get; set; } = string.Empty;
        public string ClaveUsuario { get; set; } = string.Empty;
        public DateTime? FechaCarga { get; set; }
    }
}
