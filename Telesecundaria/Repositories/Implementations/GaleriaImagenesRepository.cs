using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class GaleriaImagenesRepository : IGaleriaImagenesRepository
    {
        private readonly ApplicationDbContext _context;

        public GaleriaImagenesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GaleriaImagenesEntity>> ListarImagenesAsync()
        {
            return await _context.GaleriaImagenes.ToListAsync();
        }

        public async Task<GaleriaImagenesEntity?> ObtenerPorIdAsync(string clave)
        {
            return await _context.GaleriaImagenes
                .FirstOrDefaultAsync(g => g.ClaveImagen == clave);
        }

        public async Task<GaleriaImagenesEntity> RegistrarImagenAsync(GaleriaImagenesEntity imagen)
        {
            await _context.Database.ExecuteSqlRawAsync(
               @"CALL sp_insertar_imagen_galeria(
                    @p_nombre_archivo,
                    @p_ruta_url,
                    @p_tipo_recurso
                );",
               new NpgsqlParameter<string>("p_nombre_archivo", imagen.NombreArchivo),
               new NpgsqlParameter<string>("p_ruta_url", imagen.RutaUrl),
               new NpgsqlParameter<string>("p_tipo_recurso", imagen.TipoRecurso)
            );

            var imagenCreada = await _context.GaleriaImagenes
                .Where(g => g.RutaUrl == imagen.RutaUrl)
                .FirstOrDefaultAsync();

            return imagenCreada!;
        }

        public async Task ActualizarImagenAsync(string clave, GaleriaImagenesEntity imagen)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_actualizar_imagen_galeria(
                    @p_claveImagen,
                    @p_nombre_archivo,
                    @p_ruta_url,
                    @p_tipo_recurso
                );",
                new NpgsqlParameter<string>("p_claveImagen", clave),
                new NpgsqlParameter<string>("p_nombre_archivo", imagen.NombreArchivo),
                new NpgsqlParameter<string>("p_ruta_url", imagen.RutaUrl),
                new NpgsqlParameter<string>("p_tipo_recurso", imagen.TipoRecurso)
            );
        }

        public async Task EliminarImagenAsync(string clave)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_eliminar_imagen_galeria(@p_claveImagen);",
                new NpgsqlParameter<string>("p_claveImagen", clave)
            );
        }
    }
}
