using prueba.Dto;
using prueba.Entities;

namespace prueba.Interfaces
{
    public interface ITicketService
    {
        Task<TicketReadDto> CreateTicketAsync(TicketCreateDto dto, Guid userId);
Task<TicketReadDto?> GetTicketByIdAsync(int id);
Task<List<TicketReadDto>> GetAllTicketsAsync();
Task<bool> UpdateTicketAsync(int id, TicketUpdateDto dto);
Task<List<TicketReadDto>> GetTicketsByStatusAsync(List<TicketStatus> statuses, bool orderByPriority = false);
Task<bool> DeleteTicketAsync(int id);
Task<double> GetAverageResolutionTimeAsync();
 Task<List<TicketReadDto>> GetTicketsByAssignedUserAsync(Guid userId);
    Task<bool> UpdateTicketForAssignedUserAsync(int id, TicketUpdateDto dto, Guid currentUserId);
        Task<double> GetAverageClosedTimeByUserAsync(Guid userId);

    }
}
