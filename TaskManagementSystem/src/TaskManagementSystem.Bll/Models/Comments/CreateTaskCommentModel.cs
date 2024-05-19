namespace TaskManagementSystem.Bll.Models.Comments;

public record CreateTaskCommentModel
{
    public required long TaskId { get; init; }
    
    public required string Message { get; init; }
    
    public required long AuthorUserId { get; init; }
}