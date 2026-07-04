namespace Telesecundaria.DTOs.Publicaciones
{
    public class PublicacionResponseDTO
    {
        public string ClavePublicacion { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string? Subtitulo { get; set; }
        public string CuerpoContenido { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public DateTime FechaAparicion { get; set; }
        public DateTime? FechaRetiro { get; set; }
        public string ClaveUsuario { get; set; } = string.Empty;
        public string? ClaveConvocatoria { get; set; }
        public string? ClaveImagenPrincipal { get; set; }
        public string? ClaveImagenSecundaria { get; set; }
        public string? ClaveImagenTercera { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Destacado { get; set; }
        public bool EstatusVisible { get; set; }
    }
}
