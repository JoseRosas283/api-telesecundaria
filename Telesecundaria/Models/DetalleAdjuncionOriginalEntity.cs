using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class DetalleAdjuncionOriginalEntity
    {
        public string ClaveAdjOriginal { get; set; }
        public string ClaveDocAspirante { get; set; }
        public string RutaPdfOriginal { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual AdjuncionesOriginalesEntity AdjuncionOriginal { get; set; }
        [JsonIgnore]
        public virtual DocumentosAspiranteEntity DocumentoAspirante { get; set; }
    }
}
