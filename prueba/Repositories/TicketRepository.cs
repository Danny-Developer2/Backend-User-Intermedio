using Microsoft.EntityFrameworkCore;
using prueba.Data;
using prueba.Entities;
using prueba.Interfaces;

namespace prueba.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
        }

        public void Delete(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
        }

        public async Task<List<Ticket>> GetAllAsync()
        {
            return await _context.Tickets
                .Include(t => t.Comments)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.Comments)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public void Update(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
        }
    }
}
