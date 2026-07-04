namespace Telesecundaria.DTOs.Convocatorias
{
    public class ConvocatoriaUpdateDTO
    {
        public string ClaveConvocatoria { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Subtitulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int? CupoMaximo { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string ClaveImagen { get; set; } = string.Empty;
        public string DestacadoTexto { get; set; } = "No es destacado"; // 'Es destacado' o 'No es destacado'
    }
}
