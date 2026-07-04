namespace Telesecundaria.DTOs.Adjunciones
{
    public class AdjuncionResponseDTO
    {
        public string ClaveAdjuncion { get; set; } = string.Empty;
        public string ClaveTutor { get; set; } = string.Empty;
        public string ClaveAspirante { get; set; } = string.Empty;
        public string EstatusGral { get; set; } = string.Empty;
        public List<DocumentoAdjuntadoDTO> Documentos { get; set; } = new();
    }

    public class DocumentoAdjuntadoDTO
    {
        public string ClaveDocAspirante { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public string RutaUrl { get; set; } = string.Empty;
    }
}
