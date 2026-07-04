namespace Telesecundaria.DTOs.Modulo.Response
{
    public class ModuloResponse
    {
        public string ClaveModulo { get; set; }
        public string NombreModulo { get; set; }
        public string? Descripcion { get; set; }
        public string? UrlModulo { get; set; }
        public bool EstadoModulo { get; set; }
        public string? ClaveModuloPadre { get; set; }

        // Opcional: útil si armas un árbol de menú en el front
        public string? NombreModuloPadre { get; set; }


    }
}
