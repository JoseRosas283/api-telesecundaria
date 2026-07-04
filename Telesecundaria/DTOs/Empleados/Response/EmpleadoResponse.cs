namespace Telesecundaria.DTOs.Empleados.Response
{
    public class EmpleadoResponse
    {
        public string ClaveEmpleado { get; set; }
        public DateTime FechaContratacion { get; set; }
        public string TipoContrato { get; set; }
        public string EstatusLaboral { get; set; }
        public string Telefono { get; set; }
        public string ClaveExpediente { get; set; }
    }
}
