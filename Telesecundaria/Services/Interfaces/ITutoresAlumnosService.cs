using Telesecundaria.DTOs.TutoresAlumnos;

namespace Telesecundaria.Services.Interfaces
{
    public interface ITutoresAlumnosService
    {
        Task<IEnumerable<TutorAlumnoResponseDTO>> ListarAsync();
        Task<TutorAlumnoResponseDTO?> ObtenerPorIdAsync(string claveAlumno, string claveTutor);
        Task<TutorAlumnoResponseDTO> AsignarTutorAlumnoAsync(TutorAlumnoRequestDTO dto);
    }
}
