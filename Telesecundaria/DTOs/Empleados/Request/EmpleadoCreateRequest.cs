namespace Telesecundaria.DTOs.Empleados.Request
{
    public class EmpleadoCreateRequest
    {
        public string ClaveExpediente { get; set; }
        public string TipoContrato { get; set; }    // "Planta" o "Temporal"
        public string Telefono { get; set; }
        public DateTime FechaContratacion { get; set; }

    }
}
