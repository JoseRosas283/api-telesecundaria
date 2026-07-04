namespace Telesecundaria.DTOs.Adjunciones
{
    public class EstadoDocumentosResponseDTO
    {
        public string ClaveAspirante { get; set; } = string.Empty;
        public List<DocumentoCargadoDTO> DocumentosCargados { get; set; } = new();
        public bool TodosCompletos { get; set; }
    }

    public class DocumentoCargadoDTO
    {
        public string ClaveDocAspirante { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public string RutaUrl { get; set; } = string.Empty;
    }
}
