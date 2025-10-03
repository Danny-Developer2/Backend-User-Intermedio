using prueba.Dto;
using prueba.Entities;

namespace prueba.Interfaces
{
    public interface ITicketService
    {
        Task<Ticket> CreateTicketAsync(TicketCreateDto dto, Guid userId);
        Task<Ticket?> GetTicketByIdAsync(int id);
        Task<List<Ticket>> GetAllTicketsAsync();
        Task<bool> UpdateTicketAsync(int id, TicketUpdateDto dto);
        Task<bool> DeleteTicketAsync(int id);
    }
}
