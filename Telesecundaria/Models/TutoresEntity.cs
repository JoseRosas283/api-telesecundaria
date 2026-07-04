namespace Telesecundaria.Models
{
    public class TutoresEntity
    {
        public string ClaveTutor { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string CurpTutor { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Parentesco { get; set; }
        public bool Estado { get; set; } = true;

        // Colecciones
        public ICollection<TutoresAlumnosEntity> TutoresAlumnos { get; set; } = new List<TutoresAlumnosEntity>();
    }
}
