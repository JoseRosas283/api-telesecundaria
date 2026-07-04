namespace Telesecundaria.DTOs.Documentos.Response
{
    public class DocumentoResponse
    {
        public string ClaveDocumento { get; set; } = string.Empty;
        public string ArchivoUrl { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaSubida { get; set; }
        public string ClaveExpediente { get; set; } = string.Empty;
        public string ClaveTipoDocumento { get; set; } = string.Empty;
        public string NombreTipoDocumento { get; set; } = string.Empty;
    }

    public class DocumentosCargadosResponse
    {
        public string ClaveExpediente { get; set; } = string.Empty;
        public List<DocumentoResponse> Documentos { get; set; } = new();
    }
}
