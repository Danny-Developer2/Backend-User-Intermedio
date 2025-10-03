namespace prueba.Dto
{

    public class TicketReadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Guid CreatedByUserId { get; set; }
    public string? CreatedByName { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToName { get; set; }


    public List<TicketCommentDto> Comments { get; set; } = new();
}



}

