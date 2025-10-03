namespace prueba.Dto
{
public class TicketCommentDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? UserName { get; set; } = string.Empty;
}

}
