namespace Telesecundaria.Models
{
    public class DocumentoConEstatusProjection
    {
        public string ClaveDocAspirante { get; set; } = string.Empty;
        public string RutaArchivo { get; set; } = string.Empty;
        public string NombreTipoDocumento { get; set; } = string.Empty;
        public string ClaveAspirante { get; set; } = string.Empty;
        public string Estatus { get; set; } = string.Empty;
    }
}
