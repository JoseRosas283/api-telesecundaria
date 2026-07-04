using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.DTOs.Expedientes.Request;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class ExpedientesRepository : IExpedientesRepository
    {
        private readonly ApplicationDbContext _context;

        public ExpedientesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExpedientesEntity>> GetAllAsync()
        {
            return await _context.Expedientes
                .Include(e => e.Documentos)
                .Include(e => e.Entrega)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ExpedientesEntity?> GetByIdAsync(string claveExpediente)
        {
            return await _context.Expedientes
                .Include(e => e.Documentos)
                .Include(e => e.Entrega)
                .FirstOrDefaultAsync(e => e.ClaveExpediente == claveExpediente);
        }


        public async Task<ExpedientesEntity> CreateAsync(ExpedienteCreateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var parameters = new[]
                {
                    new NpgsqlParameter("p_nombre", NpgsqlTypes.NpgsqlDbType.Varchar)
                        {Value = request.Nombre },

                    new NpgsqlParameter("p_apellido_paterno", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = request.ApellidoPaterno },

                    new NpgsqlParameter("p_apellido_materno", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = (object?)request.ApellidoMaterno ?? DBNull.Value },

                    new NpgsqlParameter("p_curp", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = request.Curp },

                    new NpgsqlParameter("p_tipo_titular", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = request.TipoTitular },

                    new NpgsqlParameter("p_claveEntrega", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = (object?)request.ClaveEntrega ?? DBNull.Value }

                };

                await _context.Database.ExecuteSqlRawAsync(
                    @"CALL sp_insertar_expediente(
                    @p_nombre,
                    @p_apellido_paterno,
                    @p_apellido_materno,
                    @p_curp,
                    @p_tipo_titular,
                    @p_claveEntrega

                    )",
                    parameters
                    );

                await transaction.CommitAsync();

                var expedienteCreado = await _context.Expedientes
                    .Include(e => e.Documentos)
                    .Include(e => e.Entrega)
                    .FirstOrDefaultAsync(e => e.Curp == request.Curp.ToUpper().Trim());

                return expedienteCreado!;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al crear expediente: {ex.Message}", ex);
            }

        }

        public async Task UpdateAsync(string claveExpediente, ExpedienteUpdateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var parameters = new[]
                {

                    new NpgsqlParameter("p_claveExpediente", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = claveExpediente },

                    new NpgsqlParameter("p_nuevo_nombre", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = request.Nombre },

                    new NpgsqlParameter("p_nuevo_paterno", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = request.ApellidoPaterno },

                    new NpgsqlParameter("p_nuevo_materno", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = (object?)request.ApellidoMaterno ?? DBNull.Value },

                    new NpgsqlParameter("p_nueva_curp", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = request.Curp }
                };

                await _context.Database.ExecuteSqlRawAsync(
                    @"CALL sp_actualizar_expediente(
                        @p_claveExpediente,
                        @p_nuevo_nombre,
                        @p_nuevo_paterno,
                        @p_nuevo_materno,
                        @p_nueva_curp
                    )",
                    parameters
                );

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al actualizar expediente: {ex.Message}", ex);
            }

        }


        public async Task DeleteAsync(string claveExpediente)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var parameters = new[]
                {
                    new NpgsqlParameter("p_claveExpediente", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = claveExpediente }
                };

                await _context.Database.ExecuteSqlRawAsync("CALL eliminar_expediente(@p_claveExpediente)",
                 parameters
                );


                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al eliminar expediente: {ex.Message}", ex);
            }

        }
    }
}
