using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.DTOs.Publicaciones;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class PublicacionesRepository : IPublicacionesRepository
    {
        private readonly ApplicationDbContext _context;

        public PublicacionesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PublicacionesEntity>> ListarPublicacionesAsync()
        {
            return await _context.Publicaciones.ToListAsync();
        }

        public async Task<PublicacionesEntity?> ObtenerPorIdAsync(string clave)
        {
            return await _context.Publicaciones
                .FirstOrDefaultAsync(p => p.ClavePublicacion == clave);
        }

        public async Task<PublicacionesEntity> RegistrarPublicacionAsync(PublicacionRequestDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_insertar_publicacion(
                    @p_titulo,
                    @p_subtitulo,
                    @p_cuerpo,
                    @p_categoria,
                    @p_nombreUsuario,
                    @p_imgPrincipal,
                    @p_imgSecundaria,
                    @p_imgTercera,
                    @p_claveConvocatoria,
                    @p_destacado,
                    @p_estatus_visible
                );",
                new NpgsqlParameter<string>("p_titulo", dto.Titulo),
                new NpgsqlParameter("p_subtitulo", string.IsNullOrWhiteSpace(dto.Subtitulo) ? DBNull.Value : dto.Subtitulo),
                new NpgsqlParameter<string>("p_cuerpo", dto.CuerpoContenido),
                new NpgsqlParameter<string>("p_categoria", dto.Categoria),
                new NpgsqlParameter<string>("p_nombreUsuario", dto.NombreUsuario),
                new NpgsqlParameter("p_imgPrincipal", string.IsNullOrWhiteSpace(dto.ImgPrincipal) ? DBNull.Value : dto.ImgPrincipal),
                new NpgsqlParameter("p_imgSecundaria", string.IsNullOrWhiteSpace(dto.ImgSecundaria) ? DBNull.Value : dto.ImgSecundaria),
                new NpgsqlParameter("p_imgTercera", string.IsNullOrWhiteSpace(dto.ImgTercera) ? DBNull.Value : dto.ImgTercera),
                new NpgsqlParameter("p_claveConvocatoria", string.IsNullOrWhiteSpace(dto.ClaveConvocatoria) ? DBNull.Value : dto.ClaveConvocatoria),
                new NpgsqlParameter<bool>("p_destacado", dto.Destacado),
                new NpgsqlParameter<bool>("p_estatus_visible", true)
            );

            var publicacionCreada = await _context.Publicaciones
                .Where(p => p.ClaveUsuario != null)
                .OrderByDescending(p => p.FechaRegistro)
                .FirstOrDefaultAsync();

            return publicacionCreada!;
        }

        public async Task ActualizarPublicacionAsync(string clave, PublicacionesEntity publicacion)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_actualizar_publicacion(
                    @p_clavePublicacion,
                    @p_titulo,
                    @p_subtitulo,
                    @p_cuerpo_contenido,
                    @p_categoria,
                    @p_claveImagen,
                    @p_destacado,
                    @p_estatus_visible,
                    @p_fecha_retiro
                );",
                new NpgsqlParameter<string>("p_clavePublicacion", clave),
                new NpgsqlParameter<string>("p_titulo", publicacion.Titulo),
                new NpgsqlParameter<string>("p_subtitulo", publicacion.Subtitulo ?? string.Empty),
                new NpgsqlParameter<string>("p_cuerpo_contenido", publicacion.CuerpoContenido),
                new NpgsqlParameter<string>("p_categoria", publicacion.Categoria),
                new NpgsqlParameter("p_imgPrincipal", string.IsNullOrWhiteSpace(publicacion.ClaveImagenPrincipal) ? DBNull.Value : publicacion.ClaveImagenPrincipal),
                new NpgsqlParameter("p_imgSecundaria", string.IsNullOrWhiteSpace(publicacion.ClaveImagenSecundaria) ? DBNull.Value : publicacion.ClaveImagenSecundaria),
                new NpgsqlParameter("p_imgTercera", string.IsNullOrWhiteSpace(publicacion.ClaveImagenTercera) ? DBNull.Value : publicacion.ClaveImagenTercera),
                new NpgsqlParameter<bool>("p_destacado", publicacion.Destacado),
                new NpgsqlParameter<bool>("p_estatus_visible", publicacion.EstatusVisible),
                new NpgsqlParameter<DateTime?>("p_fecha_retiro", publicacion.FechaRetiro)
            );
        } 

        public async Task EliminarPublicacionAsync(string clave)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_eliminar_publicacion(@p_clavePublicacion);",
                new NpgsqlParameter<string>("p_clavePublicacion", clave)
            );
        }
    }
}
