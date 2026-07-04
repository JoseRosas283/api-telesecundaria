using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class DocumentosAspiranteEntity
    {
        public string ClaveDocAspirante { get; set; }
        public string FolioDocumento { get; set; }
        public string ValorAnalitico { get; set; }
        public string RutaArchivo { get; set; }
        public string ClaveAspirante { get; set; }
        public string ClaveTipoDocumento { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual AspirantesEntity Aspirante { get; set; }
        [JsonIgnore]
        public virtual TipoDocumentosEntity TipoDocumento { get; set; }
    }
}
