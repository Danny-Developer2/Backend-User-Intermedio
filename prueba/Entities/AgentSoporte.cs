using prueba.Entities;
public enum AgentStatus
{
    Available,
    Break,
    Lunch
}

public class SupportAgent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AgentStatus Status { get; set; } = AgentStatus.Available;

    // Tickets que tiene asignados
    public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
}
