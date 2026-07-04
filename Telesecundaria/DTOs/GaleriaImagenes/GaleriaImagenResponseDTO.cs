namespace Telesecundaria.DTOs.GaleriaImagenes
{
    public class GaleriaImagenResponseDTO
    {
        public string ClaveImagen { get; set; } = string.Empty;
        public string NombreArchivo { get; set; } = string.Empty;
        public string RutaUrl { get; set; } = string.Empty;
        public string TipoRecurso { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
    }
}
