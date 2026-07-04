namespace Telesecundaria.Models
{
    public class TipoDocumentosEntity
    {
        public string ClaveTipoDocumento { get; set; }
        public string NombreDocumento { get; set; }
        public string Area { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; } = true;
    }
}
