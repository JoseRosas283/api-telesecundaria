using Npgsql;
using Telesecundaria.DTOs.Modulo.Request;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Telesecundaria.Repositories.Implementations
{
    public class ModuloRepository : IModuloRepository
    {
        private readonly ApplicationDbContext _context;

        public ModuloRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ModulosEntity>> GetAllAsync()
        {
            return await _context.Modulos
                .Include(m => m.ModuloPadre)
                .Include(m => m.SubModulos)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ModulosEntity?> GetByIdAsync(string claveModulo)
        {
            return await _context.Modulos
                .Include(m => m.ModuloPadre)
                .Include(m => m.SubModulos)
                .FirstOrDefaultAsync(m => m.ClaveModulo == claveModulo);
        }

        public async Task<ModulosEntity?> GetByNombreAsync(string nombreModulo)
        {
            return await _context.Modulos
                .Include(m => m.ModuloPadre)
                .FirstOrDefaultAsync(m => m.NombreModulo == nombreModulo);
        }

        public async Task<ModulosEntity> CreateAsync(ModuloCreateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var parameters = new[]
                {
                    new NpgsqlParameter("p_nombre_modulo", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = request.NombreModulo },

                    new NpgsqlParameter("p_descripcion", NpgsqlTypes.NpgsqlDbType.Text)
                        { Value = (object?)request.Descripcion ?? DBNull.Value },

                    new NpgsqlParameter("p_url_modulo", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = (object?)request.UrlModulo ?? DBNull.Value },

                    new NpgsqlParameter("p_claveModuloPadre", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = (object?)request.ClaveModuloPadre ?? DBNull.Value }
                };

                await _context.Database.ExecuteSqlRawAsync(
                    @"CALL sp_insertar_modulo(
                        @p_nombre_modulo,
                        @p_descripcion,
                        @p_url_modulo,
                        @p_claveModuloPadre
                    )",
                    parameters
                );

                await transaction.CommitAsync();

                var moduloCreado = await _context.Modulos
                    .Include(m => m.ModuloPadre)
                    .FirstOrDefaultAsync(m => m.NombreModulo == request.NombreModulo);

                return moduloCreado!;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al crear módulo: {ex.Message}", ex);
            }
        }

    }
}
