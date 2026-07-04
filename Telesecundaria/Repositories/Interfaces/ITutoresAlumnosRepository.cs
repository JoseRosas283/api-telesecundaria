using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface ITutoresAlumnosRepository
    {
        Task<IEnumerable<TutoresAlumnosEntity>> ListarAsync();
        Task<TutoresAlumnosEntity?> ObtenerPorIdAsync(string claveAlumno, string claveTutor);
        Task<TutoresAlumnosEntity?> ObtenerActivoPorAlumnoAsync(string claveAlumno);
        Task AsignarTutorAlumnoAsync(string claveAlumno, string claveTutor);
    }
}
