using Olivia.Shared.Entities;

namespace Olivia.Shared.Dtos;

public class ResumeDto
{
    public required Guid ChatId { get; set; }
    public required Chat Chat { get; set; }
    public IEnumerable<Message>? Messages { get; set; } 
}
