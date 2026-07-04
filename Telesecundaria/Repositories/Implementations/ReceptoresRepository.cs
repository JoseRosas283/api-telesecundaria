using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.DTOs.Receptores;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class ReceptoresRepository : IReceptoresRepository
    {
        private readonly ApplicationDbContext _context;

        public ReceptoresRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReceptoresEntity>> ListarAsync()
        {
            return await _context.Receptores.ToListAsync();
        }

        public async Task<ReceptoresEntity?> ObtenerPorIdAsync(string clave)
        {
            return await _context.Receptores
                .FirstOrDefaultAsync(r => r.ClaveReceptor == clave);
        }

        public async Task RegistrarAsync(ReceptorRequestDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_generar_receptor(
                    @p_tipo_receptor,
                    @p_clave_referencia,
                    @p_correo
                );",
                new NpgsqlParameter<string>("p_tipo_receptor", dto.TipoReceptor),
                new NpgsqlParameter<string>("p_clave_referencia", dto.ClaveReferencia),
                new NpgsqlParameter<string>("p_correo", dto.Correo)
            );
        }

        public async Task ActualizarAsync(string claveReceptor, ReceptorUpdateDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_actualizar_receptor(
                    @p_claveReceptor,
                    @p_nuevo_correo
                );",
                new NpgsqlParameter<string>("p_claveReceptor", claveReceptor),
                new NpgsqlParameter<string>("p_nuevo_correo", dto.NuevoCorreo)
            );
        }

        public async Task EliminarAsync(string claveReceptor)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_eliminar_receptor(
                    @p_claveReceptor
                );",
                new NpgsqlParameter<string>("p_claveReceptor", claveReceptor)
            );
        }
    }
}
