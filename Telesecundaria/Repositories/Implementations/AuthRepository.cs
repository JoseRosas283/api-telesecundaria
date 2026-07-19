using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using Telesecundaria.DTOs.Auth.Internal;
using Telesecundaria.DTOs.Auth.Request;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool exito, string mensaje, string claveUsuario)> IniciarSesionAsync(LoginRequest request, string ip, string userAgent)
        {
            // SIN transaction — el SP ya maneja la suya
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
            @"CALL sp_iniciar_sesion(
            @p_nombre_usuario,
            @p_contrasena_input,
            @p_direccion_ip,
            @p_agente_usuario,
            NULL, NULL, NULL)", conn);

            cmd.Parameters.AddWithValue("p_nombre_usuario", request.NombreUsuario);
            cmd.Parameters.AddWithValue("p_contrasena_input", request.Contrasenia);
            cmd.Parameters.AddWithValue("p_direccion_ip", ip);
            cmd.Parameters.AddWithValue("p_agente_usuario", userAgent);

            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            bool exito = reader.GetBoolean(0);
            string mensaje = reader.GetString(1);
            string claveUsuario = exito ? reader.GetString(2) : string.Empty;

            return (exito, mensaje, claveUsuario);
        }

        public async Task<string> ObtenerClaveLogueoAsync(string claveUsuario)
        {
            // SELECT simple — sí aplica el patrón normal con EF
            var resultado = await _context.Logueos
                .Where(l => l.ClaveUsuario == claveUsuario
                         && l.EstatusIntento == "Exitoso"
                         && l.FechaCierre == null)
                .OrderByDescending(l => l.FechaAcceso)
                .Select(l => l.ClaveLogueo)
                .FirstOrDefaultAsync();

            return resultado ?? string.Empty;
        }

        public async Task<string> ObtenerRolAsync(string claveUsuario)
        {
            // JOIN navegando las 4 tablas con EF
            var resultado = await _context.Usuarios
                .Where(u => u.ClaveUsuario == claveUsuario)
                .SelectMany(u => u.Empleado!.EmpleadoRoles
                    .Where(er => er.FechaFin == null)
                    .Select(er => er.Rol!.NombreRol))
                .FirstOrDefaultAsync();

            return resultado ?? string.Empty;
        }

        public async Task<(bool exito, string mensaje)> CerrarSesionAsync(string claveLogueo)
        {
            // SIN transaction — igual
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "CALL sp_cerrar_sesion(@p_clave_logueo, NULL, NULL)", conn);

            cmd.Parameters.AddWithValue("p_clave_logueo", claveLogueo);

            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            bool exito = reader.GetBoolean(0);
            string mensaje = reader.GetString(1);

            return (exito, mensaje);
        }

        public async Task<UsuarioCredencialesDto?> obtenerCredencialesAsync(string nombreUsuario)
        {
            return await _context.Usuarios
                .Where(u => u.NombreUsuario == nombreUsuario)
                .Select(u => new UsuarioCredencialesDto
                {
                    ClaveUsuario = u.ClaveUsuario,
                    PasswordHash = u.Contrasenia,
                    Estado = u.Estado
                })
                .FirstOrDefaultAsync();
        }

        public async Task GuardarRefreshTokenAsync(string claveUsuario, string claveLogueo, string refreshToken, DateTime expiracion)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                @"INSERT INTO refresh_tokens (""claveUsuario"", ""claveLogueo"", token, fecha_expiracion)
          VALUES (@p_clave_usuario, @p_clave_logueo, @p_token, @p_expiracion)", conn);

            cmd.Parameters.AddWithValue("p_clave_usuario", claveUsuario);
            cmd.Parameters.AddWithValue("p_clave_logueo", claveLogueo);
            cmd.Parameters.AddWithValue("p_token", refreshToken);
            cmd.Parameters.AddWithValue("p_expiracion", expiracion);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<RefreshTokenDto?> ValidarRefreshTokenAsync(string refreshToken)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
            @"SELECT ""claveUsuario"", ""claveLogueo""
            FROM refresh_tokens
            WHERE token = @p_token
            AND revocado = FALSE
            AND fecha_expiracion > CURRENT_TIMESTAMP", conn);

            cmd.Parameters.AddWithValue("p_token", refreshToken);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new RefreshTokenDto
                {
                    ClaveUsuario = reader.GetString(0),
                    ClaveLogueo = reader.GetString(1)
                };
            }

            return null;
        }

        public async Task RevocarRefreshTokenAsync(string refreshToken)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "UPDATE refresh_tokens SET revocado = TRUE WHERE token = @p_token", conn);

            cmd.Parameters.AddWithValue("p_token", refreshToken);
            await cmd.ExecuteNonQueryAsync();
        }


    }
}
