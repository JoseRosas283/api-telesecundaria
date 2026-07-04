namespace Telesecundaria.DTOs.Convocatorias
{
    public class ConvocatoriaRequestDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Subtitulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string FechaInicio { get; set; } = string.Empty; 
        public string FechaFin { get; set; } = string.Empty;    
        public string CicloEscolar { get; set; } = string.Empty;
        public int? CupoMaximo { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string ClaveImagen { get; set; } = string.Empty;
        public bool Destacado { get; set; } = false;

        public string DestacadoTexto => Destacado ? "Es destacado" : "No es destacado";
    }
}
