using Npgsql;
using Telesecundaria.DTOs.Inscripciones;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Telesecundaria.Repositories.Implementations
{
    public class InscripcionesRepository : IInscripcionesRepository
    {
        private readonly ApplicationDbContext _context;

        public InscripcionesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InscripcionesEntity>> ListarInscripcionesAsync()
        {
            return await _context.Inscripciones.ToListAsync();
        }

        public async Task<InscripcionesEntity?> ObtenerPorIdAsync(string clave)
        {
            return await _context.Inscripciones
                .FirstOrDefaultAsync(i => i.ClaveInscripcion == clave);
        }

        public async Task RealizarInscripcionAsync(InscripcionRequestDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_realizar_inscripcion(
                    @p_claveAlumno,
                    @p_nombre_usuario
                );",
                new NpgsqlParameter<string>("p_claveAlumno", dto.ClaveAlumno),
                new NpgsqlParameter<string>("p_nombre_usuario", dto.NombreUsuario)
            );
        }
    }
}
