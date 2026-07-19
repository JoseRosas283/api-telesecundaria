namespace Telesecundaria.Models
{
    public class RefreshTokenTutorEntity
    {
        public Guid ClaveRefreshToken { get; set; }
        public string ClaveTutorAspirante { get; set; }
        public string ClaveTokenConvocatoria { get; set; }
        public string Token { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public bool Revocado { get; set; }

        // Navegación
        public virtual TutorAspiranteEntity TutorAspirante { get; set; }
        public virtual TokenConvocatoriasEntity TokenConvocatoria { get; set; }



    }
}
