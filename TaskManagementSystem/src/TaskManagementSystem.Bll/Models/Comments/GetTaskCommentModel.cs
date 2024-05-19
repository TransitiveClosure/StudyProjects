namespace TaskManagementSystem.Bll.Models.Comments;

public record GetTaskCommentModel
{
    public required long TaskId { get; init; }

    public required bool IncludeDeleted { get; init; }
}