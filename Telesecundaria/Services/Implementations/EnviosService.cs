using Telesecundaria.DTOs.Envios;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class EnviosService : IEnviosService
    {
        private readonly IEnviosRepository _repository;
        private readonly IEmailService _emailService;

        public EnviosService(IEnviosRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public async Task<IEnumerable<EnvioResponseDTO>> ListarTodosAsync()
        {
            var envios = await _repository.ListarTodosAsync();
            return envios.Select(e => MapearResponse(e));
        }

        public async Task<EnvioResponseDTO?> ObtenerPorIdAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave del envío es obligatoria.");

            var envio = await _repository.ObtenerPorIdAsync(clave);
            if (envio == null)
                return null;

            return MapearResponse(envio);
        }

        public async Task<ProcesarEnviosResponseDTO> ProcesarPendientesAsync()
        {
            var pendientes = await _repository.ListarPendientesAsync();
            int enviados = 0, fallidos = 0;

            foreach (var envio in pendientes)
            {
                try
                {
                    var ok = await _emailService.EnviarCorreoAsync(
                        envio.Destino,
                        envio.Notificacion.Titulo,
                        envio.Notificacion.Mensaje
                    );

                    if (ok)
                    {
                        await _repository.MarcarComoEnviadoAsync(envio.ClaveEnvio);
                        enviados++;
                    }
                    else
                    {
                        await _repository.MarcarComoFallidoAsync(envio.ClaveEnvio, "Fallo desconocido al enviar.");
                        fallidos++;
                    }
                }
                catch (Exception ex)
                {
                    await _repository.MarcarComoFallidoAsync(envio.ClaveEnvio, ex.Message);
                    fallidos++;
                }
            }

            return new ProcesarEnviosResponseDTO
            {
                TotalProcesados = pendientes.Count(),
                Enviados = enviados,
                Fallidos = fallidos
            };
        }

        private static EnvioResponseDTO MapearResponse(EnviosEntity e) =>
            new EnvioResponseDTO
            {
                ClaveEnvio = e.ClaveEnvio,
                ClaveNotificacion = e.ClaveNotificacion,
                Destino = e.Destino,
                ReintentoNum = e.ReintentoNum,
                Estatus = e.Estatus,
                ConfirmacionLectura = e.ConfirmacionLectura,
                FechaEnvio = e.FechaEnvio,
                ErrorLog = e.ErrorLog
            };
    }
}
