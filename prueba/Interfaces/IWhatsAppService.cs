using System.Threading.Tasks;

namespace prueba.Services
{
    public interface IWhatsAppService
    {
        Task SendMessage(string message);
        Task SendMessage(string message, string phoneNumber);
        Task SendBulkMessage(string message, List<string> phoneNumbers);
    }
}