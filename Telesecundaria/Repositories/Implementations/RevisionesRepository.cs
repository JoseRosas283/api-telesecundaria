using Npgsql;
using Telesecundaria.DTOs.Revisiones;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Telesecundaria.Repositories.Implementations
{
    public class RevisionesRepository : IRevisionesRepository
    {
        private readonly ApplicationDbContext _context;

        public RevisionesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RevisionesEntity>> ListarRevisionesAsync()
        {
            return await _context.Revisiones.ToListAsync();
        }

        public async Task<RevisionesEntity?> ObtenerPorIdAsync(string clave)
        {
            return await _context.Revisiones
                .FirstOrDefaultAsync(r => r.ClaveRevision == clave);
        }

        public async Task ProcesarRevisionAsync(RevisionRequestDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL procesar_revision(
                    @p_claveUsuario,
                    @p_observacion
                );",
                new NpgsqlParameter<string>("p_claveUsuario", dto.ClaveUsuario),
                new NpgsqlParameter<string>("p_observacion", dto.Observacion ?? string.Empty)
            );
        }

        public async Task<RevisionesEntity?> ObtenerUltimaPorUsuarioAsync(string claveUsuario)
        {
            return await _context.Revisiones
                .Where(r => r.ClaveUsuario == claveUsuario)
                .OrderByDescending(r => r.ClaveRevision)
                .FirstOrDefaultAsync();
        }

        public async Task CerrarRevisionAsync(string claveRevision)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL cerrar_revision_final(
                    @p_claveRevision
                );",
                new NpgsqlParameter<string>("p_claveRevision", claveRevision)
            );
        }
    }
}
