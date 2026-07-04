using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.DTOs.Documentos.Request;
using Telesecundaria.DTOs.DocumentosAlumnos.Request;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class DocumentosRepository : IDocumentosRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentosRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DocumentosEntity>> GetAllAsync()
        {
            return await _context.Documentos
                .Include(e => e.Expediente)
                .Include(e => e.TipoDocumento)
                .ToListAsync();
        }

        public async Task<DocumentosEntity?> GetByIdAsync(string claveDocumento)
        {
            return await _context.Documentos
                .Include(e => e.Expediente)
                .Include(e => e.TipoDocumento)
                .FirstOrDefaultAsync(e => e.ClaveDocumento == claveDocumento);
        }

        public async Task<IEnumerable<DocumentosEntity>> GetByExpedienteAsync(string claveExpediente)
        {
            return await _context.Documentos
                .Include(e => e.TipoDocumento)
                .Where(d => d.ClaveExpediente == claveExpediente)
                .ToListAsync();
        }

        public async Task<DocumentosEntity> CreateAsync(DocumentoCreateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // ── 1. Insertar documento ─────────────────────────────────────
                var paramsDoc = new[]
                {
                    new NpgsqlParameter("p_archivo_url",         NpgsqlTypes.NpgsqlDbType.Varchar) { Value = request.ArchivoUrl },
                    new NpgsqlParameter("p_claveExpediente",     NpgsqlTypes.NpgsqlDbType.Varchar) { Value = request.ClaveExpediente },
                    new NpgsqlParameter("p_nombreTipoDocumento", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = request.NombreTipoDocumento }
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "CALL sp_insertar_documento(@p_archivo_url, @p_claveExpediente, @p_nombreTipoDocumento)",
                    paramsDoc);

                var documentoCreado = await _context.Documentos
                    .Include(e => e.Expediente)
                    .Include(e => e.TipoDocumento)
                    .FirstOrDefaultAsync(d =>
                        d.ClaveExpediente == request.ClaveExpediente &&
                        d.ArchivoUrl == request.ArchivoUrl)
                    ?? throw new Exception("No se encontró el documento recién insertado.");

                // ── 2. Carga documental ───────────────────────────────────────
                var cargaExistente = await _context.CargasDocumentos
                    .FirstOrDefaultAsync(c => c.ClaveExpediente == request.ClaveExpediente);

                string claveCarga;

                if (cargaExistente is null)
                {
                    var paramsCarga = new[]
                    {
                        new NpgsqlParameter("p_clave_expediente", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = request.ClaveExpediente },
                        new NpgsqlParameter("p_clave_usuario",    NpgsqlTypes.NpgsqlDbType.Varchar) { Value = request.ClaveUsuario },
                        new NpgsqlParameter("p_clave_carga_generada", NpgsqlTypes.NpgsqlDbType.Varchar)
                        {
                            Direction = System.Data.ParameterDirection.InputOutput,
                            Size      = 18,
                            Value     = (object)DBNull.Value
                        }
                    };

                    await _context.Database.ExecuteSqlRawAsync(
                        "CALL sp_registrar_carga_documental(@p_clave_expediente, @p_clave_usuario, @p_clave_carga_generada)",
                        paramsCarga);

                    claveCarga = paramsCarga[2].Value?.ToString()
                        ?? throw new Exception("El SP no devolvió la clave de carga generada.");
                }
                else
                {
                    claveCarga = cargaExistente.ClaveCarga;
                }

                // ── 3. Detalle de carga ───────────────────────────────────────
                var paramsDetalle = new[]
                {
                    new NpgsqlParameter("p_clave_carga",     NpgsqlTypes.NpgsqlDbType.Varchar) { Value = claveCarga },
                    new NpgsqlParameter("p_clave_documento", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = documentoCreado.ClaveDocumento }
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "CALL sp_registrar_detalle_carga(@p_clave_carga, @p_clave_documento)",
                    paramsDetalle);

                await transaction.CommitAsync();
                return documentoCreado;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al subir documento: {ex.Message}", ex);
            }
        }

        public async Task<DocumentosEntity> CreateAlumnosAsync(DocumentoAlumnoCreateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // ── 1. Insertar documento ─────────────────────────────────────
                var paramsDoc = new[]
                {
                    new NpgsqlParameter("p_archivo_url",         NpgsqlTypes.NpgsqlDbType.Varchar) { Value = request.ArchivoUrl },
                    new NpgsqlParameter("p_claveExpediente",     NpgsqlTypes.NpgsqlDbType.Varchar) { Value = request.ClaveExpediente },
                    new NpgsqlParameter("p_nombreTipoDocumento", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = request.NombreTipoDocumento }
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "CALL sp_insertar_documento(@p_archivo_url, @p_claveExpediente, @p_nombreTipoDocumento)",
                    paramsDoc);

                var documentoCreado = await _context.Documentos
                    .Include(e => e.Expediente)
                    .Include(e => e.TipoDocumento)
                    .FirstOrDefaultAsync(d =>
                        d.ClaveExpediente == request.ClaveExpediente &&
                        d.ArchivoUrl == request.ArchivoUrl)
                    ?? throw new Exception("No se encontró el documento recién insertado.");

                // ── 2. Carga documental ───────────────────────────────────────
                var cargaExistente = await _context.CargasDocumentos
                    .FirstOrDefaultAsync(c => c.ClaveExpediente == request.ClaveExpediente);

                string claveCarga;

                if (cargaExistente is null)
                {
                    var paramsCarga = new[]
                    {
                        new NpgsqlParameter("p_clave_expediente",     NpgsqlTypes.NpgsqlDbType.Varchar) { Value = request.ClaveExpediente },
                        new NpgsqlParameter("p_clave_usuario",        NpgsqlTypes.NpgsqlDbType.Varchar) { Value = request.ClaveUsuario },
                        new NpgsqlParameter("p_clave_carga_generada", NpgsqlTypes.NpgsqlDbType.Varchar)
                        {
                            Direction = System.Data.ParameterDirection.InputOutput,
                            Size      = 18,
                            Value     = (object)DBNull.Value
                        }
                    };

                    await _context.Database.ExecuteSqlRawAsync(
                        "CALL sp_registrar_carga_documental(@p_clave_expediente, @p_clave_usuario, @p_clave_carga_generada)",
                        paramsCarga);

                    claveCarga = paramsCarga[2].Value?.ToString()
                        ?? throw new Exception("El SP no devolvió la clave de carga generada.");
                }
                else
                {
                    claveCarga = cargaExistente.ClaveCarga;
                }

                // ── 3. Detalle de carga ───────────────────────────────────────
                var paramsDetalle = new[]
                {
                    new NpgsqlParameter("p_clave_carga",     NpgsqlTypes.NpgsqlDbType.Varchar) { Value = claveCarga },
                    new NpgsqlParameter("p_clave_documento", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = documentoCreado.ClaveDocumento }
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "CALL sp_registrar_detalle_carga(@p_clave_carga, @p_clave_documento)",
                    paramsDetalle);

                await transaction.CommitAsync();
                return documentoCreado;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al subir documento: {ex.Message}", ex);
            }
        }
    }
}
