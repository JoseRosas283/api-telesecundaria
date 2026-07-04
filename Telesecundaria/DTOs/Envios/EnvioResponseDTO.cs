namespace Telesecundaria.DTOs.Envios
{
    public class EnvioResponseDTO
    {
        public string ClaveEnvio { get; set; } = string.Empty;
        public string ClaveNotificacion { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public int? ReintentoNum { get; set; }
        public string Estatus { get; set; } = string.Empty;
        public bool? ConfirmacionLectura { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public string? ErrorLog { get; set; }
    }
}
