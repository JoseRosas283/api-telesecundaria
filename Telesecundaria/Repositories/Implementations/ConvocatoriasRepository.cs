using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using Telesecundaria.DTOs.Convocatorias;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class ConvocatoriasRepository : IConvocatoriasRepository
    {
        private readonly ApplicationDbContext _context;

        public ConvocatoriasRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ConvocatoriasEntity>> ListarConvocatoriasAsync()
        {
            return await _context.Convocatorias.ToListAsync();
        }

        public async Task<ConvocatoriasEntity?> ObtenerPorIdAsync(string clave)
        {
            return await _context.Convocatorias
                .FirstOrDefaultAsync(c => c.ClaveConvocatoria == clave);
        }

        public async Task<ConvocatoriasEntity> RegistrarConvocatoriaAsync(ConvocatoriaRequestDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_insertar_convocatoria(
                    @p_titulo, 
                    @p_subtitulo, 
                    @p_descripcion,
                    @p_fecha_inicio_txt, 
                    @p_fecha_fin_txt,
                    @p_ciclo_escolar, 
                    @p_cupo_maximo,
                    @p_nombreUsuario, 
                    @p_claveImagen, 
                    @p_destacado_txt
                );",
                new NpgsqlParameter<string>("p_titulo", dto.Titulo),
                new NpgsqlParameter<string>("p_subtitulo", dto.Subtitulo),
                new NpgsqlParameter<string>("p_descripcion", dto.Descripcion ?? string.Empty),
                new NpgsqlParameter<string>("p_fecha_inicio_txt", dto.FechaInicio),
                new NpgsqlParameter<string>("p_fecha_fin_txt", dto.FechaFin),
                new NpgsqlParameter<string>("p_ciclo_escolar", dto.CicloEscolar),
                new NpgsqlParameter<int>("p_cupo_maximo", dto.CupoMaximo ?? 0),
                new NpgsqlParameter<string>("p_nombreUsuario", dto.NombreUsuario),
                new NpgsqlParameter<string>("p_claveImagen", dto.ClaveImagen),
                new NpgsqlParameter<string>("p_destacado_txt", dto.DestacadoTexto)
            );

            var convocatoriaCreada = await _context.Convocatorias
                .Where(c => c.Titulo == dto.Titulo && c.CicloEscolar == dto.CicloEscolar)
                .OrderByDescending(c => c.ClaveConvocatoria) 
                .FirstOrDefaultAsync();

            return convocatoriaCreada!;
        }

        public async Task ActualizarConvocatoriaAsync(string clave, ConvocatoriaUpdateDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_actualizar_convocatoria(
                    @p_claveConvocatoria,
                    @p_nombreUsuario,
                    @p_titulo,
                    @p_subtitulo,
                    @p_descripcion,
                    @p_cupo_maximo,
                    @p_claveImagen,
                    @p_destacado_txt
                );",
                new NpgsqlParameter<string>("p_claveConvocatoria", clave),
                new NpgsqlParameter<string>("p_nombreUsuario", dto.NombreUsuario),
                new NpgsqlParameter<string>("p_titulo", dto.Titulo.Trim()),
                new NpgsqlParameter<string>("p_subtitulo", dto.Subtitulo.Trim()),
                new NpgsqlParameter<string>("p_descripcion", dto.Descripcion ?? string.Empty),
                new NpgsqlParameter<int>("p_cupo_maximo", dto.CupoMaximo ?? 0),
                new NpgsqlParameter<string>("p_claveImagen", dto.ClaveImagen),
                new NpgsqlParameter<string>("p_destacado_txt", dto.DestacadoTexto)
            );
        }

        public async Task EliminarConvocatoriaAsync(string clave, string nombreUsuario)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_eliminar_convocatoria(
                    @p_claveConvocatoria,
                    @p_nombreUsuario
                );",
                new NpgsqlParameter<string>("p_claveConvocatoria", clave),
                new NpgsqlParameter<string>("p_nombreUsuario", nombreUsuario)
            );
        }
    }
}
