namespace Telesecundaria.DTOs.Empleados.Request
{
    public class EmpleadoUpdateRequest
    {
        public string TipoContrato { get; set; }    // "Planta" o "Temporal"
        public string Telefono { get; set; }
        public string EstatusLaboral { get; set; }
    }
}
