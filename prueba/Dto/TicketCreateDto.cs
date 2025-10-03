using prueba.Entities; 
namespace prueba.Dto

{
  public class TicketCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CreatedByUserId { get; set; } 
    
    // El usuario creador puede inferirse del token JWT, 
    // por lo general no lo mandas desde el cliente.
    
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
}
 
}

