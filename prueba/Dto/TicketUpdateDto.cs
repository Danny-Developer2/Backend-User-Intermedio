using prueba.Entities; 
namespace prueba.Dto
{
public class TicketUpdateDto
{
    public TicketStatus Status { get; set; }
    public TicketPriority Priority { get; set; }
    
    // Puede reasignarse a otro técnico
    public Guid? AssignedToUserId { get; set; }
    
    // Opcional: comentarios al actualizar
    public string? Comment { get; set; }
}

}

