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

        // -------------------- Crear Ticket --------------------
        public async Task<TicketReadDto> CreateTicketAsync(TicketCreateDto dto, Guid userId)
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

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

            return new TicketReadDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status.ToString(),
                Priority = ticket.Priority.ToString(),
                CreatedAt = ticket.CreatedAt,
                CreatedByUserId = ticket.CreatedByUserId,
                CreatedByName = user != null ? $"{user.FirstName} {user.LastName}" : null,
                AssignedToUserId = ticket.AssignedToUserId,
                AssignedToName = null, // se puede llenar si asignas automáticamente
                Comments = new List<TicketCommentDto>()
            };
        }

        // -------------------- Obtener Ticket por Id --------------------
        public async Task<TicketReadDto?> GetTicketByIdAsync(int id) =>
            await _unitOfWork.TicketRepository.GetByIdProjectedAsync(id);

        // -------------------- Obtener todos los Tickets --------------------
        public async Task<List<TicketReadDto>> GetAllTicketsAsync() =>
            await _unitOfWork.TicketRepository.GetAllProjectedAsync();

        // -------------------- Actualizar Ticket --------------------
        public async Task<bool> UpdateTicketAsync(int id, TicketUpdateDto dto)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(id);
            if (ticket == null) return false;

            if (ticket.Status == TicketStatus.Closed && dto.Status != TicketStatus.Closed)
                return false;

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

        // -------------------- Borrar Ticket --------------------
        public async Task<bool> DeleteTicketAsync(int id)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(id);
            if (ticket == null) return false;

            _unitOfWork.TicketRepository.Delete(ticket);
            return await _unitOfWork.Complete();
        }

        // -------------------- Filtrar Tickets por Estado --------------------
        public async Task<List<TicketReadDto>> GetTicketsByStatusAsync(
            List<TicketStatus> statuses, bool orderByPriority = false)
        {
            var tickets = await _unitOfWork.TicketRepository.GetAllProjectedAsync();
            var filtered = tickets.Where(t => statuses.Contains(Enum.Parse<TicketStatus>(t.Status!)));

            if (orderByPriority)
                filtered = filtered.OrderBy(t => Enum.Parse<TicketPriority>(t.Priority!));

            return filtered.ToList();
        }

        // -------------------- Promedio de tiempo hasta resolución --------------------
        public async Task<double> GetAverageResolutionTimeAsync()
        {
            var tickets = await _unitOfWork.TicketRepository.GetAllAsync();
            var resolvedTickets = tickets.Where(t => t.ResolvedAt.HasValue);

            if (!resolvedTickets.Any()) return 0;

            return resolvedTickets.Average(t => (t.ResolvedAt.Value - t.CreatedAt).TotalHours);
        }

        // -------------------- Promedio de tiempo hasta cierre por usuario --------------------
        public async Task<double> GetAverageClosedTimeByUserAsync(Guid userId)
        {
            var tickets = await _unitOfWork.TicketRepository.GetAllAsync();
            var userTickets = tickets.Where(t => t.AssignedToUserId == userId && t.ClosedAt.HasValue);

            if (!userTickets.Any()) return 0;

            return userTickets.Average(t => (t.ClosedAt.Value - t.CreatedAt).TotalHours);
        }

        // -------------------- Tickets asignados al usuario --------------------
        public async Task<List<TicketReadDto>> GetTicketsByAssignedUserAsync(Guid userId)
        {
            var tickets = await _unitOfWork.TicketRepository.GetAllProjectedAsync();
            return tickets.Where(t => t.AssignedToUserId == userId).ToList();
        }

        // -------------------- Actualizar ticket solo por usuario asignado --------------------
        public async Task<bool> UpdateTicketForAssignedUserAsync(int id, TicketUpdateDto dto, Guid currentUserId)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(id);
            if (ticket == null) return false;

            if (ticket.AssignedToUserId != currentUserId) return false;

            if (dto.Status != TicketStatus.Closed && dto.Status != TicketStatus.Cancelled)
                return false;

            ticket.Status = dto.Status;
            ticket.Priority = dto.Priority;

            if (!string.IsNullOrEmpty(dto.Comment))
            {
                ticket.Comments.Add(new TicketComment
                {
                    UserId = currentUserId,
                    Message = dto.Comment,
                    CreatedAt = DateTime.UtcNow
                });
            }

            _unitOfWork.TicketRepository.Update(ticket);
            return await _unitOfWork.Complete();
        }

        // -------------------- Asignar ticket a agente disponible --------------------
        // public async Task<SupportAgent?> AssignTicketToAvailableAgentAsync(Ticket ticket)
        // {
        //     var agents = await _unitOfWork.SupportAgentRepository.GetAllAsync();

        //     var availableAgents = agents
        //         .Where(a => a.Status == AgentStatus.Available
        //                     && a.AssignedTickets.Count(t => t.Status == TicketStatus.InProgress) < 2)
        //         .ToList();

        //     if (!availableAgents.Any())
        //         return null;

        //     var agentToAssign = availableAgents
        //         .OrderBy(a => a.AssignedTickets.Count(t => t.Status == TicketStatus.InProgress))
        //         .First();

        //     ticket.AssignedToUserId = agentToAssign.Id;
        //     ticket.Status = TicketStatus.InProgress;

        //     _unitOfWork.TicketRepository.Update(ticket);
        //     await _unitOfWork.Complete();

        //     return agentToAssign;
        // }
    }
}
