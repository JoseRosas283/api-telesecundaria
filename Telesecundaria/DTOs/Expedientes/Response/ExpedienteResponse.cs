namespace Telesecundaria.DTOs.Expedientes.Response
{
    public class ExpedienteResponse
    {
        public string ClaveExpediente { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Curp { get; set; }
        public string TipoTitular { get; set; }
        public string? ClaveEntrega { get; set; }
    }
}
