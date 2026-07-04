namespace Telesecundaria.Models
{
    public class LogueosEntity
    {
        public string ClaveLogueo { get; set; }

        // Nullable: puede haber intento con usuario inexistente
        public string? ClaveUsuario { get; set; }

        public DateTime FechaAcceso { get; set; }
        public string EstatusIntento { get; set; }         // CHECK en BD
        public string DireccionIp { get; set; } = "0.0.0.0";
        public string? AgenteUsuario { get; set; }
        public DateTime? FechaCierre { get; set; }

        // Navegación
        public UsuariosEntity? Usuario { get; set; }
    }
}
