using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
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
    }
}
