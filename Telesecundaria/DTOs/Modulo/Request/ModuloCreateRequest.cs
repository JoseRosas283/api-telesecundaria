namespace Telesecundaria.DTOs.Modulo.Request
{
    public class ModuloCreateRequest
    {

        public string NombreModulo { get; set; }
        public string? Descripcion { get; set; }
        public string? UrlModulo { get; set; }
        public string? ClaveModuloPadre { get; set; }
    }
}
