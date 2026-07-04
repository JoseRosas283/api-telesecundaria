namespace Telesecundaria.DTOs.TipoDocumentos
{
    public class TipoDocumentoResponseDTO
    {
        public string ClaveTipoDocumento { get; set; } = string.Empty;
        public string NombreDocumento { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Estado { get; set; }
    }
}
