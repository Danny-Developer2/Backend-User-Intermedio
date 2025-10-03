using prueba.Entities; 

namespace prueba.Dto
{
public class TicketResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public TicketStatus Status { get; set; }
    public TicketPriority Priority { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public UserDTO CreatedBy { get; set; } = null!;
    public UserDTO? AssignedTo { get; set; }
    
    public List<TicketCommentDto> Comments { get; set; } = new();
}

}


