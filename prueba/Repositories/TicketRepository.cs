using Microsoft.EntityFrameworkCore;
using prueba.Data;
using prueba.Entities;
using prueba.Interfaces;
using prueba.Dto;

namespace prueba.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }

        // Agregar un ticket
        public async Task AddAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
        }

        // Eliminar un ticket
        public void Delete(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
        }

        // Actualizar un ticket
        public void Update(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
        }

        // Obtener un ticket por Id (entidad completa con Includes)
        public async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // Obtener un ticket por Id proyectado a DTO (evita exponer info sensible)
        public async Task<TicketReadDto?> GetByIdProjectedAsync(int id)
        {
            return await _context.Tickets
                .Where(t => t.Id == id)
                .Select(t => new TicketReadDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString(),
                    CreatedAt = t.CreatedAt,
                    CreatedByUserId = t.CreatedByUserId,
                    CreatedByName = t.CreatedBy != null ? (t.CreatedBy.FirstName + " " + t.CreatedBy.LastName) : null,
                    Comments = t.Comments.Select(c => new TicketCommentDto
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        UserName = c.User != null ? (c.User.FirstName + " " + c.User.LastName) : null,
                        Message = c.Message,
                        CreatedAt = c.CreatedAt
                    }).ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        // Obtener todos los tickets (entidades completas)
        public async Task<List<Ticket>> GetAllAsync()
        {
            return await _context.Tickets
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .ToListAsync();
        }

        // Obtener todos los tickets proyectados a DTO (recomendado para API)
        public async Task<List<TicketReadDto>> GetAllProjectedAsync()
        {
            return await _context.Tickets
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Select(t => new TicketReadDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString(),
                    CreatedAt = t.CreatedAt,
                    CreatedByUserId = t.CreatedByUserId,
                    CreatedByName = t.CreatedBy != null ? (t.CreatedBy.FirstName + " " + t.CreatedBy.LastName) : null,
                    Comments = t.Comments.Select(c => new TicketCommentDto
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        UserName = c.User != null ? (c.User.FirstName + " " + c.User.LastName) : null,
                        Message = c.Message,
                        CreatedAt = c.CreatedAt
                    }).ToList()
                })
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
