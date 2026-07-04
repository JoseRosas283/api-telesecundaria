namespace Telesecundaria.DTOs.GaleriaImagenes
{
    public class GaleriaImagenRequestDTO
    {
        public string NombreArchivo { get; set; } = string.Empty;
        public IFormFile Imagen { get; set; } = null!;
        public string TipoRecurso { get; set; } = string.Empty;
    }
}
