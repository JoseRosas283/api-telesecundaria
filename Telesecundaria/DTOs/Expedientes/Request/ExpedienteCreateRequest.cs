namespace Telesecundaria.DTOs.Expedientes.Request
{
    public class ExpedienteCreateRequest
    {
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Curp { get; set; }
        public string TipoTitular { get; set; }
        public string? ClaveEntrega { get; set; }
    }
}
