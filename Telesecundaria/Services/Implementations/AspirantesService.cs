using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Aspirantes;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class AspirantesService : IAspirantesService
    {
        private readonly IAspirantesRepository _repository;

        private static readonly string[] EstatusPermitidos =
        {
            "En proceso", "Aceptado", "Rechazado"
        };

        public AspirantesService(IAspirantesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AspiranteResponseDTO>> ListarAspirantesAsync()
        {
            var aspirantes = await _repository.ListarAspirantesAsync();
            return aspirantes.Select(a => MapearResponse(a));
        }

        public async Task<AspiranteResponseDTO?> ObtenerPorIdAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave del aspirante es obligatoria.");

            var aspirante = await _repository.ObtenerPorIdAsync(clave);
            if (aspirante == null)
                return null;

            return MapearResponse(aspirante);
        }

        public async Task<AspiranteResponseDTO?> ObtenerPorCurpAsync(string curp)
        {
            if (string.IsNullOrWhiteSpace(curp))
                throw new ArgumentException("El CURP es obligatorio.");

            var aspirante = await _repository.ObtenerPorCurpAsync(curp.Trim().ToUpper());
            if (aspirante == null)
                return null;

            return MapearResponse(aspirante);
        }

        public async Task<IEnumerable<AspiranteResponseDTO>> ObtenerPorConvocatoriaAsync(string claveConvocatoria)
        {
            if (string.IsNullOrWhiteSpace(claveConvocatoria))
                throw new ArgumentException("La clave de la convocatoria es obligatoria.");

            var aspirantes = await _repository.ObtenerPorConvocatoriaAsync(claveConvocatoria);
            return aspirantes.Select(a => MapearResponse(a));
        }

        public async Task<AspiranteResponseDTO> RegistrarAspiranteAsync(AspiranteRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre del aspirante es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.ApellidoPaterno))
                throw new ArgumentException("El apellido paterno es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.Curp) || dto.Curp.Length != 18)
                throw new ArgumentException("El CURP debe tener exactamente 18 caracteres.");
            if (string.IsNullOrWhiteSpace(dto.EscuelaProcedencia))
                throw new ArgumentException("La escuela de procedencia es obligatoria.");
            if (dto.PromedioPrimaria <= 0 || dto.PromedioPrimaria > 10)
                throw new ArgumentException("El promedio debe estar entre 0.1 y 10.");
            if (string.IsNullOrWhiteSpace(dto.ClaveTutorAspirante))
                throw new ArgumentException("La clave del tutor aspirante es obligatoria.");

            dto.Nombre = dto.Nombre.Trim();
            dto.ApellidoPaterno = dto.ApellidoPaterno.Trim();
            dto.ApellidoMaterno = dto.ApellidoMaterno?.Trim();
            dto.Curp = dto.Curp.Trim().ToUpper();
            dto.EscuelaProcedencia = dto.EscuelaProcedencia.Trim();
            dto.CurpHermano = dto.CurpHermano?.Trim().ToUpper();
            dto.ClaveTutorAspirante = dto.ClaveTutorAspirante.Trim();

            // Validar CURP único
            var curpExistente = await _repository.ObtenerPorCurpAsync(dto.Curp.Trim().ToUpper());
            if (curpExistente != null)
                throw new InvalidOperationException($"Ya existe un aspirante registrado con el CURP: {dto.Curp}");

            var aspiranteCreado = await _repository.RegistrarAspiranteAsync(dto);
            return MapearResponse(aspiranteCreado);
        }

        public async Task ActualizarAspiranteAsync(string clave, AspiranteUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave del aspirante es requerida.");

            var existe = await _repository.ObtenerPorIdAsync(clave);
            if (existe == null)
                throw new InvalidOperationException($"El aspirante con clave {clave} no existe.");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre del aspirante es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.ApellidoPaterno))
                throw new ArgumentException("El apellido paterno es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.Curp) || dto.Curp.Trim().Length != 18)
                throw new ArgumentException("La CURP debe tener exactamente 18 caracteres.");
            if (string.IsNullOrWhiteSpace(dto.EscuelaProcedencia))
                throw new ArgumentException("La escuela de procedencia es obligatoria.");
            if (dto.PromedioPrimaria <= 0 || dto.PromedioPrimaria > 10)
                throw new ArgumentException("El promedio de primaria debe estar en un rango de 0.1 a 10.0.");

            dto.Nombre = dto.Nombre.Trim();
            dto.ApellidoPaterno = dto.ApellidoPaterno.Trim();
            dto.ApellidoMaterno = string.IsNullOrWhiteSpace(dto.ApellidoMaterno) ? null : dto.ApellidoMaterno.Trim();
            dto.Curp = dto.Curp.Trim().ToUpper();
            dto.EscuelaProcedencia = dto.EscuelaProcedencia.Trim();

            await _repository.ActualizarAspiranteAsync(clave, dto);
        }

        public async Task EliminarAspiranteAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("Se requiere una clave válida para eliminar al aspirante.");

            var existe = await _repository.ObtenerPorIdAsync(clave);
            if (existe == null)
                throw new InvalidOperationException($"El aspirante con clave {clave} no existe en los registros actuales.");

            await _repository.EliminarAspiranteAsync(clave);
        }

        private static AspiranteResponseDTO MapearResponse(AspirantesEntity a) =>
            new AspiranteResponseDTO
            {
                ClaveAspirante = a.ClaveAspirante,
                Nombre = a.Nombre,
                ApellidoPaterno = a.ApellidoPaterno,
                ApellidoMaterno = a.ApellidoMaterno,
                Curp = a.Curp,
                EscuelaProcedencia = a.EscuelaProcedencia,
                PromedioPrimaria = a.PromedioPrimaria,
                TieneDiscapacidad = a.TieneDiscapacidad,
                NombreEnfermedad = a.NombreEnfermedad,
                HermanoPlantel = a.HermanoPlantel,
                CurpHermano = a.CurpHermano,
                EstatusAspirante = a.EstatusAspirante,
                ClaveConvocatoria = a.ClaveConvocatoria,
                ClaveTutorAspirante = a.ClaveTutorAspirante,
                Estado = a.Estado
            };
    }
}
