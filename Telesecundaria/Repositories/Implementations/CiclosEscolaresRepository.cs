using Npgsql;
using Telesecundaria.DTOs.CiclosEscolares;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Telesecundaria.Repositories.Implementations
{
    public class CiclosEscolaresRepository : ICiclosEscolaresRepository
    {
        private readonly ApplicationDbContext _context;

        public CiclosEscolaresRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CiclosEscolaresEntity>> ListarCiclosAsync()
        {
            return await _context.CiclosEscolares.ToListAsync();
        }

        public async Task<CiclosEscolaresEntity?> ObtenerPorIdAsync(string clave)
        {
            return await _context.CiclosEscolares
                .FirstOrDefaultAsync(c => c.ClaveCiclo == clave);
        }

        public async Task AbrirCicloAsync(CicloEscolarRequestDTO dto)
        {
            var fechaInicio = DateOnly.ParseExact(dto.FechaInicio, "dd/MM/yyyy");
            var fechaFin = DateOnly.ParseExact(dto.FechaFin, "dd/MM/yyyy");

            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_apertura_ciclo_escolar(
                    @p_fecha_inicio,
                    @p_fecha_fin
                );",
                new NpgsqlParameter<DateOnly>("p_fecha_inicio", fechaInicio),  
                new NpgsqlParameter<DateOnly>("p_fecha_fin", fechaFin)
            );
        }
    }
}
