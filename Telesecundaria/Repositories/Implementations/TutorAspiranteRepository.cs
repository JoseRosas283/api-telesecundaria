using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.DTOs.TutorAspirante;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class TutorAspiranteRepository : ITutorAspiranteRepository
    {
        private readonly ApplicationDbContext _context;

        public TutorAspiranteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TutorAspiranteEntity>> ListarTutoresAsync()
        {
            return await _context.TutorAspirante.ToListAsync();
        }

        public async Task<TutorAspiranteEntity?> ObtenerPorIdAsync(string clave)
        {
            return await _context.TutorAspirante
                .FirstOrDefaultAsync(t => t.ClaveTutorAspirante == clave);
        }

        public async Task<TutorAspiranteEntity?> ObtenerPorCurpAsync(string curp)
        {
            return await _context.TutorAspirante
                .FirstOrDefaultAsync(t => t.CurpTutor == curp);
        }

        public async Task<TutorAspiranteEntity> RegistrarTutorAsync(TutorAspiranteRequestDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_insertar_tutor_con_direccion(
                    @p_nombre,
                    @p_apellido_paterno,
                    @p_apellido_materno,
                    @p_curp_tutor,
                    @p_telefono,
                    @p_correo,
                    @p_parentesco,
                    @p_contrasena,
                    @p_calle_numero,
                    @p_colonia,
                    @p_codigo_postal,
                    @p_municipio
                );",
                new NpgsqlParameter<string>("p_nombre", dto.Nombre),
                new NpgsqlParameter<string>("p_apellido_paterno", dto.ApellidoPaterno),
                new NpgsqlParameter<string>("p_apellido_materno", dto.ApellidoMaterno ?? string.Empty),
                new NpgsqlParameter<string>("p_curp_tutor", dto.CurpTutor),
                new NpgsqlParameter<string>("p_telefono", dto.Telefono),
                new NpgsqlParameter<string>("p_correo", dto.Correo),
                new NpgsqlParameter<string>("p_parentesco", dto.Parentesco),
                new NpgsqlParameter<string>("p_contrasena", dto.Contrasena),
                new NpgsqlParameter<string>("p_calle_numero", dto.CalleNumero),
                new NpgsqlParameter<string>("p_colonia", dto.Colonia),
                new NpgsqlParameter<string>("p_codigo_postal", dto.CodigoPostal),
                new NpgsqlParameter<string>("p_municipio", dto.Municipio)
            );

            var tutorCreado = await _context.TutorAspirante
            .Where(t => t.CurpTutor == dto.CurpTutor)
            .FirstOrDefaultAsync();

            return tutorCreado!;
        }

        public async Task ActualizarTutorAsync(string clave, TutorAspiranteUpdateDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_actualizar_tutor_Aspirante(
                    @p_claveTutor,
                    @p_nombre,
                    @p_apellido_paterno,
                    @p_apellido_materno,
                    @p_curp_tutor,
                    @p_telefono,
                    @p_correo,
                    @p_parentesco,
                    @p_contrasena,
                    @p_calle_numero,
                    @p_colonia,
                    @p_codigo_postal,
                    @p_municipio
                );",
                new NpgsqlParameter<string>("p_claveTutor", clave),
                new NpgsqlParameter<string>("p_nombre", dto.Nombre),
                new NpgsqlParameter<string>("p_apellido_paterno", dto.ApellidoPaterno),
                new NpgsqlParameter<string>("p_apellido_materno", dto.ApellidoMaterno ?? string.Empty),
                new NpgsqlParameter<string>("p_curp_tutor", dto.CurpTutor),
                new NpgsqlParameter<string>("p_telefono", dto.Telefono),
                new NpgsqlParameter<string>("p_correo", dto.Correo),
                new NpgsqlParameter<string>("p_parentesco", dto.Parentesco),
                new NpgsqlParameter<string>("p_contrasena", dto.Contrasena),
                new NpgsqlParameter<string>("p_calle_numero", dto.CalleNumero),
                new NpgsqlParameter<string>("p_colonia", dto.Colonia),
                new NpgsqlParameter<string>("p_codigo_postal", dto.CodigoPostal),
                new NpgsqlParameter<string>("p_municipio", dto.Municipio)
            );
        }

        public async Task EliminarTutorAsync(string clave)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_eliminar_tutor_aspirante(@p_claveTutor);",
                new NpgsqlParameter<string>("p_claveTutor", clave)
            );
        }
    }
}
