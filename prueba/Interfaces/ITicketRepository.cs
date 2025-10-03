using prueba.Entities;
using prueba.Dto;

namespace prueba.Interfaces
{
    public interface ITicketRepository
    {
        Task<Ticket?> GetByIdAsync(int id);
        Task<List<Ticket>> GetAllAsync();
        Task<TicketReadDto?> GetByIdProjectedAsync(int id);
        Task<List<TicketReadDto>> GetAllProjectedAsync();
        Task AddAsync(Ticket ticket);
        void Update(Ticket ticket);
        void Delete(Ticket ticket);
    }
}
