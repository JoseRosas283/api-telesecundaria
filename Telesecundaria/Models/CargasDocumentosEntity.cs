namespace Telesecundaria.Models
{
    public class CargasDocumentosEntity
    {
        public string ClaveCarga { get; set; }

        // UNIQUE en BD: un expediente solo tiene una carga
        public string ClaveExpediente { get; set; }
        public string ClaveUsuario { get; set; }

        public DateTime FechaCarga { get; set; }
        public string? Observaciones { get; set; }
        public string EstatusValidacion { get; set; } = "En Proceso";

        // Navegación
        public ExpedientesEntity Expediente { get; set; }
        public UsuariosEntity Usuario { get; set; }
        public ICollection<DetalleCargaEntity> DetalleCarga { get; set; } = new List<DetalleCargaEntity>();
    }
}
