using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Configuration;

namespace prueba.Services
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromNumber;
        private readonly string _defaultToNumber;

        public WhatsAppService(IConfiguration configuration)
        {
            _accountSid = configuration["Twilio:AccountSid"]!;
            _authToken = configuration["Twilio:AuthToken"]!;
            _fromNumber = configuration["Twilio:WhatsAppFromNumber"]!;
            _defaultToNumber = configuration["Twilio:DefaultToNumber"]!;

            TwilioClient.Init(_accountSid, _authToken);
        }

        public async Task SendMessage(string message)
        {
            await SendMessage(message, _defaultToNumber);
        }

        public async Task SendMessage(string message, string phoneNumber)
        {
            try
            {
                var messageResource = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber($"whatsapp:{_fromNumber}"),
                    to: new Twilio.Types.PhoneNumber($"whatsapp:{phoneNumber}")
                );
            }
            catch (Exception ex)
            {
                // Handle or log the error appropriately
                throw new Exception($"Failed to send WhatsApp message: {ex.Message}");
            }
        }

        public async Task SendBulkMessage(string message, List<string> phoneNumbers)
        {
            var tasks = phoneNumbers.Select(phoneNumber => SendMessage(message, phoneNumber));
            await Task.WhenAll(tasks);
        }
    }
}