
namespace prueba.Entities
{
    public enum TicketStatus
    {
        Open,        // Ticket creado, esperando atención
        InProgress,  // Un técnico ya lo está trabajando
        Resolved,    // Solucionado, pendiente de que el usuario confirme
        Closed,      // Cerrado definitivamente
        Cancelled    // Cancelado por el usuario o admin
    }

    public class Ticket
    {
        public int Id { get; set; }

        // Estado actual del ticket
        public TicketStatus Status { get; set; } = TicketStatus.Open;

        // Título y descripción del problema
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Usuario que creó el ticket
        public Guid CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = null!;

        // Técnico asignado (puede ser null si aún no hay asignación)
        public Guid? AssignedToUserId { get; set; }

        public User? AssignedTo { get; set; }

        // Fechas importantes
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        // Prioridad del ticket
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;

        // Historial de comentarios o acciones
        public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
    }

    public enum TicketPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class TicketComment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; } = null!;
        
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
