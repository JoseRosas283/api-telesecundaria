namespace Telesecundaria.DTOs.GaleriaImagenes
{
    public class GaleriaImagenUpdateDTO
    {
        public string NombreArchivo { get; set; } = string.Empty;
        public IFormFile? Imagen { get; set; }
        public string TipoRecurso { get; set; } = string.Empty;
    }
}
