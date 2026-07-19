using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.DTOs.Aspirantes;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class AspirantesRepository : IAspirantesRepository
    {
        private readonly ApplicationDbContext _context;

        public AspirantesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AspirantesEntity>> ListarAspirantesAsync()
        {
            return await _context.Aspirantes.ToListAsync();
        }

        public async Task<AspirantesEntity?> ObtenerPorIdAsync(string clave)
        {
            return await _context.Aspirantes
                .FirstOrDefaultAsync(a => a.ClaveAspirante == clave);
        }

        public async Task<AspirantesEntity?> ObtenerPorCurpAsync(string curp)
        {
            return await _context.Aspirantes
                .FirstOrDefaultAsync(a => a.Curp == curp);
        }

        public async Task<IEnumerable<AspirantesEntity>> ObtenerPorConvocatoriaAsync(string claveConvocatoria)
        {
            return await _context.Aspirantes
                .Where(a => a.ClaveConvocatoria == claveConvocatoria)
                .ToListAsync();
        }

        public async Task<AspirantesEntity> RegistrarAspiranteAsync(AspiranteRequestDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_insertar_aspirante(
                    @p_nombre,
                    @p_apellido_paterno,
                    @p_apellido_materno,
                    @p_curp,
                    @p_escuela_procedencia,
                    @p_promedio_primaria,
                    @p_discapacidad_txt,
                    @p_nombre_enfermedad,
                    @p_hermano_txt,
                    @p_curp_hermano,
                    @p_claveTutorAspirante
                );",
                new NpgsqlParameter<string>("p_nombre", dto.Nombre),
                new NpgsqlParameter<string>("p_apellido_paterno", dto.ApellidoPaterno),
                new NpgsqlParameter("p_apellido_materno",
                    string.IsNullOrWhiteSpace(dto.ApellidoMaterno) ? DBNull.Value : (object)dto.ApellidoMaterno),
                new NpgsqlParameter<string>("p_curp", dto.Curp),
                new NpgsqlParameter<string>("p_escuela_procedencia", dto.EscuelaProcedencia),
                new NpgsqlParameter<decimal>("p_promedio_primaria", dto.PromedioPrimaria),
                new NpgsqlParameter<string>("p_discapacidad_txt", dto.DiscapacidadTexto),
                new NpgsqlParameter("p_nombre_enfermedad",
                    string.IsNullOrWhiteSpace(dto.NombreEnfermedad) ? DBNull.Value : (object)dto.NombreEnfermedad),
                new NpgsqlParameter<string>("p_hermano_txt", dto.HermanoTexto),
                new NpgsqlParameter("p_curp_hermano",
                    string.IsNullOrWhiteSpace(dto.CurpHermano) ? DBNull.Value : (object)dto.CurpHermano),
                new NpgsqlParameter<string>("p_claveTutorAspirante", dto.ClaveTutorAspirante)
            );

            var aspiranteCreado = await _context.Aspirantes
                .Where(a => a.Curp == dto.Curp)
                .FirstOrDefaultAsync();

            return aspiranteCreado!;
        }

        public async Task ActualizarAspiranteAsync(string clave, AspiranteUpdateDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_actualizar_aspirante(
                     @p_claveAspirante,
                     @p_nombre,
                     @p_apellido_paterno,
                     @p_apellido_materno,
                     @p_curp,
                     @p_escuela_procedencia,
                     @p_promedio_primaria
                 );",
        new NpgsqlParameter<string>("p_claveAspirante", clave),
        new NpgsqlParameter<string>("p_nombre", dto.Nombre),
        new NpgsqlParameter<string>("p_apellido_paterno", dto.ApellidoPaterno),
        new NpgsqlParameter("p_apellido_materno",
            string.IsNullOrWhiteSpace(dto.ApellidoMaterno) ? DBNull.Value : (object)dto.ApellidoMaterno),
        new NpgsqlParameter<string>("p_curp", dto.Curp),
        new NpgsqlParameter<string>("p_escuela_procedencia", dto.EscuelaProcedencia),
        new NpgsqlParameter<decimal>("p_promedio_primaria", dto.PromedioPrimaria)
            );
        }

        public async Task EliminarAspiranteAsync(string clave)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_eliminar_aspirante(@p_claveAspirante);",
                new NpgsqlParameter<string>("p_claveAspirante", clave)
            );
        }
    }
}
