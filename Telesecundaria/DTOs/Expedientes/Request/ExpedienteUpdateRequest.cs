namespace Telesecundaria.DTOs.Expedientes.Request
{
    public class ExpedienteUpdateRequest
    {
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string Curp { get; set; }
    }
}
