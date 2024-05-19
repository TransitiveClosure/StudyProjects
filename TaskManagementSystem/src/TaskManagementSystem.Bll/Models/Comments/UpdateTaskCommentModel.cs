namespace TaskManagementSystem.Bll.Models.Comments;

public class UpdateTaskCommentModel
{
    public required long CommentId { get; init; }
    
    public required string Message { get; init; }
}