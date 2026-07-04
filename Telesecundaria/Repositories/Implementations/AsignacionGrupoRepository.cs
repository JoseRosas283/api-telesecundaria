using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.DTOs.AsignacionGrupo;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class AsignacionGrupoRepository : IAsignacionGrupoRepository
    {
        private readonly ApplicationDbContext _context;

        public AsignacionGrupoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AsignacionGrupoEntity>> ListarAsignacionesAsync()
        {
            return await _context.AsignacionGrupo.ToListAsync();
        }

        public async Task<AsignacionGrupoEntity?> ObtenerPorIdAsync(string clave)
        {
            return await _context.AsignacionGrupo
                .FirstOrDefaultAsync(a => a.ClaveAsignacion == clave);
        }

        public async Task AsignarAlumnoGrupoAsync(AsignacionGrupoRequestDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_asignar_alumno_grupo(
                    @p_claveAlumno,
                    @p_claveGrupo,
                    @p_claveUsuario
                );",
                new NpgsqlParameter<string>("p_claveAlumno", dto.ClaveAlumno),
                new NpgsqlParameter<string>("p_claveGrupo", dto.ClaveGrupo),
                new NpgsqlParameter<string>("p_claveUsuario", dto.ClaveUsuario)
            );
        }
    }
}
