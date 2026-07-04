namespace Telesecundaria.DTOs.Publicaciones
{
    public class PublicacionUpdateDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string? Subtitulo { get; set; }
        public string CuerpoContenido { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string? ImgPrincipal { get; set; }
        public string? ImgSecundaria { get; set; }
        public string? ImgTercera { get; set; }
        public bool Destacado { get; set; } = false;
        public bool EstatusVisible { get; set; } = true;
        public DateTime? FechaRetiro { get; set; }
    }
}
