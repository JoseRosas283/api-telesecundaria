namespace Telesecundaria.DTOs.EmpleadoRol.Response
{
    public class EmpleadoRolResponse
    {
        public string ClaveEmpleado { get; set; }
        public string ClaveRol { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
