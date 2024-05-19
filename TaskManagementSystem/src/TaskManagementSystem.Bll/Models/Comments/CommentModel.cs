namespace TaskManagementSystem.Bll.Models.Comments;

public record CommentModel
{
    public required long Id { get; init; }
    
    public required long TaskId { get; init; }

    public required string Comment { get; init; }

    public required bool IsDeleted { get; init; }

    public required DateTimeOffset At { get; init; }
}