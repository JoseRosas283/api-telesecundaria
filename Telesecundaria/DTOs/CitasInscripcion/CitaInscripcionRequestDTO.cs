namespace Telesecundaria.DTOs.CitasInscripcion
{
    public class CitaInscripcionRequestDTO
    {
        public string ClaveRevision { get; set; }
        public DateTime FechaCita { get; set; }
        public TimeSpan HoraCita { get; set; }
    }
}
