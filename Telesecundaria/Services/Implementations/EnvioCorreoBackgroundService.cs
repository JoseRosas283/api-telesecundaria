using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class EnvioCorreoBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<EnvioCorreoBackgroundService> _logger;

        public EnvioCorreoBackgroundService(IServiceProvider services, ILogger<EnvioCorreoBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var enviosService = scope.ServiceProvider.GetRequiredService<IEnviosService>();
                        var resultado = await enviosService.ProcesarPendientesAsync();

                        if (resultado.TotalProcesados > 0)
                        {
                            _logger.LogInformation(
                                "Envíos procesados: {Total}, Enviados: {Enviados}, Fallidos: {Fallidos}",
                                resultado.TotalProcesados, resultado.Enviados, resultado.Fallidos);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en el procesamiento automático de envíos.");
                }

                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
            }
        }
    }
}
