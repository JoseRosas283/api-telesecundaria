using System.Globalization;
using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Convocatorias;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class ConvocatoriasService : IConvocatoriasService
    {
        private readonly IConvocatoriasRepository _repository;

        public ConvocatoriasService(IConvocatoriasRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ConvocatoriaResponseDTO>> ListarConvocatoriasAsync()
        {
            var convocatorias = await _repository.ListarConvocatoriasAsync();
            return convocatorias.Select(c => MapearResponse(c));
        }

        public async Task<ConvocatoriaResponseDTO?> ObtenerPorIdAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave de la convocatoria es obligatoria.");

            var convocatoria = await _repository.ObtenerPorIdAsync(clave);
            if (convocatoria == null)
                return null;

            return MapearResponse(convocatoria);
        }

        public async Task<ConvocatoriaResponseDTO> RegistrarConvocatoriaAsync(ConvocatoriaRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Titulo))
                throw new ArgumentException("El título de la convocatoria es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.Subtitulo))
                throw new ArgumentException("El subtítulo es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.NombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.ClaveImagen))
                throw new ArgumentException("La clave de imagen es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.CicloEscolar))
                throw new ArgumentException("El ciclo escolar es obligatorio.");
            if (dto.CupoMaximo.HasValue && dto.CupoMaximo <= 0)
                throw new ArgumentException("El cupo máximo debe ser mayor a cero.");
            if (string.IsNullOrWhiteSpace(dto.FechaInicio) || string.IsNullOrWhiteSpace(dto.FechaFin))
                throw new ArgumentException("Las fechas de inicio y fin son obligatorias.");

            if (!DateTime.TryParseExact(dto.FechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                throw new ArgumentException("La fecha de inicio no tiene un formato válido o no es una fecha real. Use DD/MM/YYYY.");
            }

            if (!DateTime.TryParseExact(dto.FechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                throw new ArgumentException("La fecha de fin no tiene un formato válido o no es una fecha real. Use DD/MM/YYYY.");
            }

            var convocatoriaCreada = await _repository.RegistrarConvocatoriaAsync(dto);
            return MapearResponse(convocatoriaCreada);
        }

        public async Task ActualizarConvocatoriaAsync(string clave, ConvocatoriaUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave de la convocatoria es requerida en la URL.");

            dto.ClaveConvocatoria = clave;

            var existe = await _repository.ObtenerPorIdAsync(clave);
            if (existe == null)
                throw new InvalidOperationException($"La convocatoria con clave {clave} no existe.");

            if (string.IsNullOrWhiteSpace(dto.Titulo))
                throw new ArgumentException("El título es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.Subtitulo))
                throw new ArgumentException("El subtítulo de la publicación web es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.NombreUsuario))
                throw new ArgumentException("El nombre del usuario que modifica es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.ClaveImagen))
                throw new ArgumentException("La clave de imagen para la publicación es obligatoria.");
            if (dto.CupoMaximo.HasValue && dto.CupoMaximo <= 0)
                throw new ArgumentException("El cupo máximo debe ser mayor a cero.");

            await _repository.ActualizarConvocatoriaAsync(clave, dto);
        }

        public async Task EliminarConvocatoriaAsync(string clave, string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("Se requiere una clave de convocatoria válida.");

            if (string.IsNullOrWhiteSpace(nombreUsuario))
                throw new ArgumentException("Se requiere el nombre del usuario para dar de baja.");

            var existe = await _repository.ObtenerPorIdAsync(clave);
            if (existe == null)
                throw new InvalidOperationException($"La convocatoria con clave {clave} no existe.");

            await _repository.EliminarConvocatoriaAsync(clave, nombreUsuario);
        }

        private static ConvocatoriaResponseDTO MapearResponse(ConvocatoriasEntity c) =>
            new ConvocatoriaResponseDTO
            {
                ClaveConvocatoria = c.ClaveConvocatoria,
                Titulo = c.Titulo,
                Descripcion = c.Descripcion,
                FechaInicio = c.FechaInicio,
                FechaFin = c.FechaFin,
                CicloEscolar = c.CicloEscolar,
                CupoMaximo = c.CupoMaximo,
                CupoDisponible = c.CupoDisponible,
                Activacion = c.Activacion,
                Estado = c.Estado,
                FechaRegistro = c.FechaRegistro
            };
    }
}
