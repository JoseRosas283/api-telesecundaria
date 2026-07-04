namespace Telesecundaria.Models
{
    public class DetalleCargaEntity
    {
        // PK compuesta: (ClaveCarga, ClaveDocumento)
        public string ClaveCarga { get; set; }
        public string ClaveDocumento { get; set; }

        public string ArchivoUrl { get; set; }
        public DateTime FechaSubida { get; set; }

        // Navegación
        public CargasDocumentosEntity CargaDocumento { get; set; }
        public DocumentosEntity Documento { get; set; }
    }
}
