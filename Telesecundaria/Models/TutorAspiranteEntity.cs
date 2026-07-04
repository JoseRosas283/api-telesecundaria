namespace Telesecundaria.Models
{
    public class TutorAspiranteEntity
    {
        public string ClaveTutorAspirante { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string CurpTutor { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Parentesco { get; set; }
        public bool Estado { get; set; } = true;
        public string Contrasena { get; set; } = "Temporal123";

        // Colecciones
        public ICollection<AspirantesEntity> Aspirantes { get; set; } = new List<AspirantesEntity>();
        public ICollection<DireccionesEntity> Direcciones { get; set; } = new List<DireccionesEntity>();
        public ICollection<AdjuncionesEntity> Adjunciones { get; set; } = new List<AdjuncionesEntity>();
    }
}
