namespace Olivia.Shared.Dtos;

public class NewMessageDto
{
    public Guid ChatId { get; set; }
    public required string Content { get; set; }
}
