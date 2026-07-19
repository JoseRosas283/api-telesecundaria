namespace Telesecundaria.DTOs.AuthTutor.Response
{
    public class LoginTutorResponse
    {
        public string Token { get; set; } = string.Empty;
        public string ClaveToken { get; set; } = string.Empty;
        public string ClaveTutorAspirante { get; set; } = string.Empty;
        public string NombreTutor { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonIgnore]
        public string RefreshToken {  get; set; } = string.Empty;
    }
}
