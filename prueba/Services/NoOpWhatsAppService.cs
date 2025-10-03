using prueba.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace prueba.Services
{
public class NoOpWhatsAppService : IWhatsAppService 
{
    private readonly ILogger<NoOpWhatsAppService> _logger;

    public NoOpWhatsAppService(ILogger<NoOpWhatsAppService> logger)
    {
        _logger = logger;
    }

    public Task SendMessage(string message)
    {
        _logger.LogInformation("Twilio está deshabilitado. Mensaje no enviado: {message}", message);
        return Task.CompletedTask;
    }

    public Task SendMessage(string message, string phoneNumber)
    {
        _logger.LogInformation("Twilio está deshabilitado. Mensaje no enviado a {phoneNumber}: {message}", phoneNumber, message);
        return Task.CompletedTask;
    }

    public Task SendBulkMessage(string message, List<string> phoneNumbers)
    {
        foreach (var number in phoneNumbers)
        {
            _logger.LogInformation("Twilio está deshabilitado. Mensaje no enviado a {phoneNumber}: {message}", number, message);
        }
        return Task.CompletedTask;
    }
}

}
