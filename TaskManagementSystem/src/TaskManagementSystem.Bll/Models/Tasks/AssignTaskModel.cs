namespace TaskManagementSystem.Bll.Models.Tasks;

public record AssignTaskModel
{
    public required long TaskId { get; init; }
    
    public required long AssignToUserId { get; init; }
    
    public required long UserId { get; init; }
}