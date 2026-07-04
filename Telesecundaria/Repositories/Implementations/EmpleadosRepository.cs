using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.DTOs.Empleados.Request;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class EmpleadosRepository : IEmpleadosRepository
    {
        private readonly ApplicationDbContext _context;


        public EmpleadosRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmpleadosEntity>> GetAllAsync()
        {
            return await _context.Empleados
                .Include(e => e.Expediente)
                .Include(e => e.EmpleadoRoles)
                    .ThenInclude(er => er.Rol)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<EmpleadosEntity?> GetByIdAsync(string claveEmpleado)
        {
            return await _context.Empleados
                .Include(e => e.Expediente)
                .Include(e => e.EmpleadoRoles)
                    .ThenInclude(er => er.Rol)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.ClaveEmpleado == claveEmpleado);
        }

        public async Task<EmpleadosEntity> CreateAsync(EmpleadoCreateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var parameters = new[]
                {
                    new NpgsqlParameter("p_claveExpediente", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = request.ClaveExpediente },

                    new NpgsqlParameter("p_tipo_contrato", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = request.TipoContrato },

                    new NpgsqlParameter("p_telefono", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = (object?)request.Telefono ?? DBNull.Value },

                    new NpgsqlParameter("p_fecha_contratacion", NpgsqlTypes.NpgsqlDbType.Date)
                        { Value = request.FechaContratacion }
                };

                await _context.Database.ExecuteSqlRawAsync(
                    @"CALL sp_insertar_empleado(
                        @p_claveExpediente,
                        @p_tipo_contrato,
                        @p_telefono,
                        @p_fecha_contratacion
                    )",
                    parameters
                );

                await transaction.CommitAsync();


                var empleadoCreado = await _context.Empleados
                    .Include(e => e.Expediente)
                    .Include(e => e.EmpleadoRoles)
                        .ThenInclude(er => er.Rol)
                    .FirstOrDefaultAsync(e => e.ClaveExpediente == request.ClaveExpediente);

                return empleadoCreado!;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al crear empleado: {ex.Message}", ex);
            }
        }

        public async Task UpdateAsync(string claveEmpleado, EmpleadoUpdateRequest request)
        {
            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.ClaveEmpleado == claveEmpleado);

            if (empleado == null) return;


            empleado.TipoContrato = request.TipoContrato?.Trim();
            empleado.EstatusLaboral = request.EstatusLaboral?.Trim();
            empleado.Telefono = request.Telefono?.Trim();

            _context.Empleados.Update(empleado);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string claveEmpleado)
        {
            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.ClaveEmpleado == claveEmpleado);

            if (empleado == null) return;

            _context.Empleados.Remove(empleado);
            await _context.SaveChangesAsync();
        }
    }
}
