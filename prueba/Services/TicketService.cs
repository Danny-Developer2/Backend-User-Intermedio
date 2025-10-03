using prueba.Entities;
using prueba.Dto;
using prueba.Interfaces;

namespace prueba.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TicketService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Ticket> CreateTicketAsync(TicketCreateDto dto, Guid userId)
        {
            var ticket = new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                CreatedByUserId = userId,
                Status = TicketStatus.Open,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.TicketRepository.AddAsync(ticket);
            await _unitOfWork.Complete();
            return ticket;
        }

        public async Task<Ticket?> GetTicketByIdAsync(int id) =>
            await _unitOfWork.TicketRepository.GetByIdAsync(id);

        public async Task<List<Ticket>> GetAllTicketsAsync() =>
            await _unitOfWork.TicketRepository.GetAllAsync();

        public async Task<bool> UpdateTicketAsync(int id, TicketUpdateDto dto)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(id);
            if (ticket == null) return false;

            ticket.Status = dto.Status;
            ticket.Priority = dto.Priority;

            if (dto.AssignedToUserId.HasValue)
                ticket.AssignedToUserId = dto.AssignedToUserId;

            if (!string.IsNullOrEmpty(dto.Comment))
            {
                ticket.Comments.Add(new TicketComment
                {
                    UserId = dto.AssignedToUserId ?? ticket.CreatedByUserId,
                    Message = dto.Comment,
                    CreatedAt = DateTime.UtcNow
                });
            }

            _unitOfWork.TicketRepository.Update(ticket);
            return await _unitOfWork.Complete();
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(id);
            if (ticket == null) return false;

            _unitOfWork.TicketRepository.Delete(ticket);
            return await _unitOfWork.Complete();
        }
    }
}
