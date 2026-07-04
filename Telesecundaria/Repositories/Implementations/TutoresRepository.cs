using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.DTOs.Tutores;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class TutoresRepository : ITutoresRepository
    {
        private readonly ApplicationDbContext _context;

        public TutoresRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TutoresEntity>> ListarTutoresAsync()
        {
            return await _context.Tutores.ToListAsync();
        }

        public async Task<TutoresEntity?> ObtenerPorIdAsync(string clave)
        {
            return await _context.Tutores
                .FirstOrDefaultAsync(t => t.ClaveTutor == clave);
        }

        public async Task<TutoresEntity?> ObtenerPorCurpAsync(string curp)
        {
            return await _context.Tutores
                .FirstOrDefaultAsync(t => t.CurpTutor == curp);
        }

        public async Task RegistrarTutorAsync(TutorRequestDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_insertar_tutor(
                    @p_nombre,
                    @p_apellido_paterno,
                    @p_apellido_materno,
                    @p_curp_tutor,
                    @p_telefono,
                    @p_correo,
                    @p_parentesco
                );",
                new NpgsqlParameter<string>("p_nombre", dto.Nombre),
                new NpgsqlParameter<string>("p_apellido_paterno", dto.ApellidoPaterno),
                new NpgsqlParameter<string>("p_apellido_materno", dto.ApellidoMaterno ?? string.Empty),
                new NpgsqlParameter<string>("p_curp_tutor", dto.CurpTutor),
                new NpgsqlParameter<string>("p_telefono", dto.Telefono),
                new NpgsqlParameter<string>("p_correo", dto.Correo),
                new NpgsqlParameter<string>("p_parentesco", dto.Parentesco)
            );
        }

        public async Task ActualizarTutorAsync(string claveTutor, TutorUpdateDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_actualizar_tutor(
                    @p_claveTutor,
                    @p_nombre,
                    @p_apellido_paterno,
                    @p_apellido_materno,
                    @p_curp_tutor,
                    @p_telefono,
                    @p_correo,
                    @p_parentesco
                );",
                new NpgsqlParameter<string>("p_claveTutor", claveTutor),
                new NpgsqlParameter<string>("p_nombre", dto.Nombre),
                new NpgsqlParameter<string>("p_apellido_paterno", dto.ApellidoPaterno),
                new NpgsqlParameter<string>("p_apellido_materno", dto.ApellidoMaterno ?? string.Empty),
                new NpgsqlParameter<string>("p_curp_tutor", dto.CurpTutor),
                new NpgsqlParameter<string>("p_telefono", dto.Telefono),
                new NpgsqlParameter<string>("p_correo", dto.Correo),
                new NpgsqlParameter<string>("p_parentesco", dto.Parentesco)
            );
        }

        public async Task EliminarTutorAsync(string claveTutor)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_eliminar_tutor(
                    @p_claveTutor
                );",
                new NpgsqlParameter<string>("p_claveTutor", claveTutor)
            );
        }
    }
}
