using Npgsql;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Telesecundaria.Repositories.Implementations
{
    public class TutoresAlumnosRepository : ITutoresAlumnosRepository
    {
        private readonly ApplicationDbContext _context;

        public TutoresAlumnosRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TutoresAlumnosEntity>> ListarAsync()
        {
            return await _context.TutoresAlumnos.ToListAsync();
        }

        public async Task<TutoresAlumnosEntity?> ObtenerPorIdAsync(string claveAlumno, string claveTutor)
        {
            return await _context.TutoresAlumnos
                .FirstOrDefaultAsync(ta => ta.ClaveAlumno == claveAlumno && ta.ClaveTutor == claveTutor);
        }

        public async Task<TutoresAlumnosEntity?> ObtenerActivoPorAlumnoAsync(string claveAlumno)
        {
            return await _context.TutoresAlumnos
                .Where(ta => ta.ClaveAlumno == claveAlumno && ta.FechaBaja == null)
                .OrderByDescending(ta => ta.FechaInicio)
                .FirstOrDefaultAsync();
        }

        public async Task AsignarTutorAlumnoAsync(string claveAlumno, string claveTutor)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_asignar_tutor_alumno(
                    @p_claveAlumno,
                    @p_claveTutor
                );",
                new NpgsqlParameter<string>("p_claveAlumno", claveAlumno),
                new NpgsqlParameter<string>("p_claveTutor", claveTutor)
            );
        }
    }
}
