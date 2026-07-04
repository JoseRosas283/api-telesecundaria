using Telesecundaria.DTOs.Pagos;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class PagosService : IPagosService
    {
        private readonly IPagosRepository _repository;

        public PagosService(IPagosRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PagoResponseDTO>> ListarPagosAsync()
        {
            var pagos = await _repository.ListarPagosAsync();
            return pagos.Select(p => MapearResponse(p));
        }

        public async Task<PagoResponseDTO?> ObtenerPorIdAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave del pago es obligatoria.");

            var pago = await _repository.ObtenerPorIdAsync(clave);
            if (pago == null)
                return null;

            return MapearResponse(pago);
        }

        public async Task RegistrarPagoAsync(PagoRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ClaveTutor))
                throw new ArgumentException("La clave del tutor es obligatoria.");

            if (string.IsNullOrWhiteSpace(dto.NombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            if (dto.Monto <= 0)
                throw new ArgumentException("El monto debe ser mayor a cero.");

            var metodosValidos = new[] { "Efectivo", "Transferencia", "Deposito" };
            if (string.IsNullOrWhiteSpace(dto.MetodoPago) || !metodosValidos.Contains(dto.MetodoPago))
                throw new ArgumentException("El método de pago debe ser 'Efectivo', 'Transferencia' o 'Deposito'.");

            await _repository.RegistrarPagoAsync(dto);
        }

        private static PagoResponseDTO MapearResponse(PagosEntity p) =>
            new PagoResponseDTO
            {
                ClavePago = p.ClavePago,
                ClaveTutor = p.ClaveTutor,
                ClaveCiclo = p.ClaveCiclo,
                Monto = p.Monto,
                FechaPago = p.FechaPago,
                MetodoPago = p.MetodoPago,
                ComprobantePago = p.ComprobantePago,
                Referencia = p.Referencia,
                EstadoPago = p.EstadoPago
            };
    }
}
